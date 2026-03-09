using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Optical.Enums;

namespace Optical.API.Library.Device
{
    [SupportedOSPlatform("windows")]
    /// <summary>
    /// プロセッサーの制御情報を共有します。
    /// </summary>
    public class CpuComponents
    {
        #region Fields
        private Process? process;
        private ProcessThread? thread;
        #endregion // Fields

        #region Constructors
        /// <summary>
        /// <see cref="CpuComponents"/>クラスの新しいインスタンスを生成し、指定された現在の実行単位に関連付けます。
        /// </summary>
        /// <param name="affinityTarget">対象実行単位</param>
        public CpuComponents(ExecutableUnit affinityTarget)
        {
            initializeTargetId(affinityTarget);
        }

        /// <summary>
        /// <see cref="CpuComponents"/>クラスの新しいインスタンスを生成し、指定した既存のプロセスリソースに関連付けます。
        /// </summary>
        /// <param name="processId">プロセス識別子</param>
        /// <exception cref="ArgumentException"><paramref name="processId"/>パラメーターで指定されたプロセスが実行されていません。 識別子の有効期限が切れている可能性があります。</exception>
        /// <exception cref="InvalidOperationException">プロセスはこのオブジェクトによって開始されませんでした。</exception>
        public CpuComponents(int processId)
        {
            try
            {
                process = Process.GetProcessById(processId);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        /// <summary>
        /// <see cref="CpuComponents"/>クラスの新しいインスタンスを生成し、指定した既存のスレッドリソースに関連付けます。
        /// </summary>
        /// <param name="threadId">スレッド識別子</param>
        public CpuComponents(uint threadId)
        {
            AffinityTarget = ExecutableUnit.Thread;

            try
            {
                initializeThread(threadId);
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        /// <summary>
        /// <see cref="CpuComponents"/>クラスの新しいインスタンスを生成し、指定した既存のプロセスリソースに関連付けます。
        /// </summary>
        /// <param name="process">実行中プロセス</param>
        public CpuComponents(Process process)
        {
            AffinityTarget = ExecutableUnit.Process;

            this.process = process;
        }

        /// <summary>
        /// <see cref="CpuComponents"/>クラスの新しいインスタンスを生成し、指定した既存のスレッドリソースに関連付けます。
        /// </summary>
        /// <param name="thread">実行中スレッド</param>
        public CpuComponents(ProcessThread thread)
        {
            AffinityTarget = ExecutableUnit.Thread;

            this.thread = thread;
        }
        #endregion // Constructors

        #region Enums
        /// <summary>
        /// プログラムの実行単位
        /// </summary>
        public enum ExecutableUnit
        {
            /// <summary>
            /// プロセス
            /// </summary>
            Process,

            /// <summary>
            /// スレッド
            /// </summary>
            Thread
        }

        /// <summary>
        /// 実行単位の優先度を指定します。
        /// </summary>
        public enum ExecutablePriorityLevel
        {
            /// <summary>
            /// アイドル優先度を指定します。これは、関連するProcessPriorityClassの値に関係なく、すべてのスレッドの中で可能な限り低い優先度の値です。
            /// </summary>
            Idle = -15,

            /// <summary>
            /// 最も低い優先度を指定します。これは、関連するProcessPriorityClassの通常の優先度より2ステップ低くなります。
            /// </summary>
            Lowest = -2,

            /// <summary>
            /// 関連するProcessPriorityClassの通常の優先度より1ステップ上のステップを指定します。
            /// </summary>
            BelowNormal = -1,

            /// <summary>
            /// 関連するProcessPriorityClassの通常の優先度を指定します。
            /// </summary>
            Normal = 0,

            /// <summary>
            /// 関連するProcessPriorityClassの通常の優先度より1ステップ上のステップを指定します。
            /// </summary>
            AboveNormal = 1,

            /// <summary>
            /// 最高の優先度を指定します。これは、関連するProcessPriorityClassの通常の優先度より2ステップ上です。
            /// </summary>
            Highest = 2,

            /// <summary>
            /// タイムクリティカルな優先度を指定します。これは、関連するProcessPriorityClassの値に関係なく、すべてのスレッドの中で最も高い優先度です。
            /// </summary>
            TimeCritical = 15
        }
        #endregion // Enums

        #region Properties
        /// <summary>
        /// プロセッサーの関係性を設定する実行単位。
        /// </summary>
        public ExecutableUnit AffinityTarget { get; set; }
        #endregion // Properties

        #region Methods
        private void initializeProcess()
        {
            if (process != null)
            {
                return;
            }

            process = Process.GetCurrentProcess();
        }

        private void initializeTargetId(ExecutableUnit target)
        {
            if (target == ExecutableUnit.Process)
            {
                initializeProcess();
            }
            else if (target == ExecutableUnit.Thread)
            {
                initializeThread();
            }
        }

        private void initializeThread(uint threadId = uint.MaxValue)
        {
            if (thread != null)
            {
                return;
            }

            uint currentId = (threadId == uint.MaxValue) ? SafeNativeMethods.GetCurrentThreadId() : threadId;
            var process = Process.GetCurrentProcess();
            foreach (ProcessThread thread in process.Threads)
            {
                if (thread.Id == currentId)
                {
                    this.thread = thread;
                    break;
                }
            }

            if ((threadId != uint.MaxValue) && (thread == null))
            {
                throw new ArgumentException("パラメーターで指定されたプロセスが実行されていません。 識別子の有効期限が切れている可能性があります。", nameof(threadId));
            }
        }

        private void processAffinity(int processorIndex, bool enable)
        {
            if (process is null)
            {
                return;
            }

            ulong bitFlag = (ulong)(1 << processorIndex);
            ulong affinityMask = (ulong)process.ProcessorAffinity;
            if (enable)
            {
                affinityMask |= bitFlag;
            }
            else
            {
                affinityMask &= ~bitFlag;
            }

            if (affinityMask == 0)
            {
                return;
            }

            process.ProcessorAffinity = (IntPtr)affinityMask;
        }

        private void processAffinity(ProcessorId id)
        {
            if (process is null)
            {
                return;
            }

            ulong affinityMask = 0;
            foreach (ProcessorId flag in Enum.GetValues(typeof(ProcessorId)))
            {
                if (id.HasFlag(flag))
                {
                    affinityMask += (ulong)flag;
                }
            }

            process.ProcessorAffinity = (IntPtr)affinityMask;
        }

        private void threadAffinity(int processorIndex, bool enable)
        {
            if ((process is null) || (thread is null))
            {
                return;
            }

            ulong bitFlag = (ulong)(1 << processorIndex);
            ulong affinityMask = (ulong)process.ProcessorAffinity;
            if (enable)
            {
                affinityMask |= bitFlag;
            }
            else
            {
                affinityMask &= ~bitFlag;
            }

            if (affinityMask == 0)
            {
                return;
            }

            thread.ProcessorAffinity = (IntPtr)affinityMask;
        }

        private void threadAffinity(ProcessorId id)
        {
            if (thread is null)
            {
                return;
            }

            ulong affinityMask = 0;
            foreach (ProcessorId flag in Enum.GetValues(typeof(ProcessorId)))
            {
                if (id.HasFlag(flag))
                {
                    affinityMask += (ulong)(flag);
                }
            }

            thread.ProcessorAffinity = (IntPtr)affinityMask;
        }

        /// <summary>
        /// 現在のスレッドの関係性を設定します。
        /// </summary>
        /// <param name="id"></param>
        public void CurrentThreadAffinity(ProcessorId id)
        {
            if (thread is null)
            {
                return;
            }

            uint currentId = SafeNativeMethods.GetCurrentThreadId();
            var process = Process.GetCurrentProcess();
            foreach (ProcessThread thread in process.Threads)
            {
                if (thread.Id == currentId)
                {
                    this.thread = thread;
                    break;
                }
            }

            ulong affinityMask = 0;
            foreach (ProcessorId flag in Enum.GetValues(typeof(ProcessorId)))
            {
                if (id.HasFlag(flag))
                {
                    affinityMask += (ulong)(flag);
                }
            }

            thread.ProcessorAffinity = (IntPtr)affinityMask;
        }

        /// <summary>
        /// 現在のスレッドの関係性を初期状態に戻します。
        /// </summary>
        public void ClearCurrentThreadAffinity()
        {
            if (thread is null)
            {
                return;
            }

            uint currentId = SafeNativeMethods.GetCurrentThreadId();
            var process = Process.GetCurrentProcess();
            foreach (ProcessThread thread in process.Threads)
            {
                if (thread.Id == currentId)
                {
                    this.thread = thread;
                    break;
                }
            }

            ulong affinityMask = 0;
            for (int index = 0; index < Environment.ProcessorCount; index++)
            {
                affinityMask += (ulong)(1 << index);
            }

            thread.ProcessorAffinity = (IntPtr)affinityMask;
        }

        /// <summary>
        /// 実行単位の優先度を設定します。
        /// </summary>
        /// <param name="level">実行優先度</param>
        public void PriorityLevel(ExecutablePriorityLevel level)
        {
            if (AffinityTarget == ExecutableUnit.Process)
            {
                if (process is null)
                {
                    return;
                }

                switch (level)
                {
                    case ExecutablePriorityLevel.Idle:
                        process.PriorityClass = ProcessPriorityClass.Idle;
                        break;
                    case ExecutablePriorityLevel.Lowest:
                    case ExecutablePriorityLevel.BelowNormal:
                        process.PriorityClass = ProcessPriorityClass.BelowNormal;
                        break;
                    case ExecutablePriorityLevel.Normal:
                        process.PriorityClass = ProcessPriorityClass.Normal;
                        break;
                    case ExecutablePriorityLevel.AboveNormal:
                        process.PriorityClass = ProcessPriorityClass.AboveNormal;
                        break;
                    case ExecutablePriorityLevel.Highest:
                        process.PriorityClass = ProcessPriorityClass.High;
                        break;
                    case ExecutablePriorityLevel.TimeCritical:
                        process.PriorityClass = ProcessPriorityClass.RealTime;
                        break;
                    default:
                        break;
                }
            }
            else if (AffinityTarget == ExecutableUnit.Thread)
            {
                if (thread is null)
                {
                    return;
                }

                switch (level)
                {
                    case ExecutablePriorityLevel.Idle:
                        thread.PriorityLevel = ThreadPriorityLevel.Idle;
                        break;
                    case ExecutablePriorityLevel.Lowest:
                        thread.PriorityLevel = ThreadPriorityLevel.Lowest;
                        break;
                    case ExecutablePriorityLevel.BelowNormal:
                        thread.PriorityLevel = ThreadPriorityLevel.BelowNormal;
                        break;
                    case ExecutablePriorityLevel.Normal:
                        thread.PriorityLevel = ThreadPriorityLevel.Normal;
                        break;
                    case ExecutablePriorityLevel.AboveNormal:
                        thread.PriorityLevel = ThreadPriorityLevel.AboveNormal;
                        break;
                    case ExecutablePriorityLevel.Highest:
                        thread.PriorityLevel = ThreadPriorityLevel.Highest;
                        break;
                    case ExecutablePriorityLevel.TimeCritical:
                        thread.PriorityLevel = ThreadPriorityLevel.TimeCritical;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 指定されたプロセッサーの関係性を設定します。
        /// </summary>
        /// <param name="processorIndex">プロセッサー識別子の添字</param>
        /// <param name="enable"></param>
        public void ProcessorAffinity(int processorIndex, bool enable)
        {
            if ((processorIndex < 0) || (processorIndex >= Environment.ProcessorCount))
            {
                return;
            }

            if (AffinityTarget == ExecutableUnit.Process)
            {
                processAffinity(processorIndex, enable);
            }
            else if (AffinityTarget == ExecutableUnit.Thread)
            {
                threadAffinity(processorIndex, enable);
            }
        }

        /// <summary>
        /// プロセッサーの関係性を設定します。
        /// </summary>
        /// <param name="id">プロセッサー識別子</param>
        public void ProcessorAffinity(ProcessorId id)
        {
            if (id <= 0)
            {
                return;
            }

            if ((ulong)id >= (ulong)(1 << Environment.ProcessorCount))
            {
                return;
            }

            if (AffinityTarget == ExecutableUnit.Process)
            {
                processAffinity(id);
            }
            else if (AffinityTarget == ExecutableUnit.Thread)
            {
                threadAffinity(id);
            }
        }
        #endregion // Methods
    }

    internal static class SafeNativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();
    }
}
