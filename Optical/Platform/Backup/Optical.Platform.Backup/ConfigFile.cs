using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace Optical.Platform.Backup
{
    /// <summary>
    /// 各クラス毎のパラメータをコンフィギュレーションファイルに記録します
    /// </summary>
    public class ConfigFile
    {
        #region Fields
        /// <summary>
        /// パラメーター登録用テーブル
        /// </summary>
        private readonly Dictionary<string, object> registerTable;

        /// <summary>
        /// パラメーター登録・参照用型テーブル
        /// </summary>
        private readonly Dictionary<string, Type> typeTable;

        /// <summary>
        /// パラメーター参照用テーブル
        /// </summary>
        private Dictionary<string, object> referenceTable;
        #endregion // Fields

        #region Constructors
        /// <summary>
        /// <see cref="ConfigFile"/>の新しいインスタンスを生成します。
        /// </summary>
        public ConfigFile()
        {
            registerTable = [];
            typeTable = [];
            referenceTable = [];
        }
        #endregion // Constructors

        #region Methods
        /// <summary>
        /// 登録されたパラメーターをシステムファイルを書き込みます
        /// </summary>
        /// <param name="filePath">書き込み先のファイル</param>
        /// <exception cref="ArgumentNullException"><paramref name="filePath"/>がNull又は空。</exception>
        /// <exception cref="InvalidDataContractException"><paramref name="filePath"/>にファイルがない。</exception>
        /// <exception cref="SerializationException">XMLのシリアル化に失敗。</exception>
        public void Save(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path is null or empty.");
            }

            if (registerTable.Count < 1)
            {
                throw new InvalidDataContractException("パラメーターが登録されていません。");
            }

            var xmlFile = new XmlFile(registerTable);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath.AsSpan()).ToString());
                var xml = new DataContractSerializer(typeof(XmlFile), [.. typeTable.Values]);
                using var writer = XmlWriter.Create(filePath);
                xml.WriteObject(writer, xmlFile);
            }
            catch (InvalidDataContractException)
            {
                throw;
            }
            catch (SerializationException)
            {
                throw;
            }

            return;
        }

        /// <summary>
        /// システムファイルを読み込みます
        /// </summary>
        /// <param name="filePath">読み込み元のファイルパス。</param>
        /// <exception cref="ArgumentNullException"><paramref name="filePath"/>がNull又は空です。</exception>
        /// <exception cref="InvalidDataContractException"><see cref="Register{T}(string, T)"/>で<see langword="Class"/>が登録されていない。</exception>
        /// <exception cref="FileNotFoundException"><paramref name="filePath"/>にファイルが存在しない。</exception>
        /// <exception cref="SerializationException">XMLの逆シリアル化に失敗。</exception>
        /// <remarks>パラメーター読み込み前に、<see cref="Register{T}(string, T)"/>で対象となる<see langword="Class"/>を登録する必要があります。</remarks>
        public void Load(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (typeTable.Count < 1)
            {
                throw new InvalidDataContractException("読み込み対象のパラメーターが登録されていません。");
            }

            try
            {
                var xml = new DataContractSerializer(typeof(XmlFile), [.. typeTable.Values]);
                using var reader = XmlReader.Create(filePath);
                if (xml.ReadObject(reader) is not XmlFile xmlFile)
                {
                    return;
                }

                referenceTable = xmlFile.ReadParameter();
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (SerializationException)
            {
                throw;
            }

            return;
        }

        /// <summary>
        /// パラメーターをシステムファイルに登録します
        /// </summary>
        /// <typeparam name="T">登録するパラメーターの型</typeparam>
        /// <param name="key">登録するパラメーターのキー。
        /// <para>・<paramref name="key"/>と<paramref name="parameter"/>は1対1でなければならない。</para>
        /// <para>・<paramref name="key"/>は、名前空間を含めたクラス名+[.インスタンス名]とする。</para>
        /// <example>
        /// <code>
        /// var key = "System.Platform.Backup.ConfigFile.userConfig";
        /// </code>
        /// </example>
        /// </param>
        /// <param name="parameter">登録するパラメーター。
        /// <para>・<paramref name="parameter"/>はクラス形式で登録しなければならない。値型は登録不可。</para>
        /// <para>・<paramref name="parameter"/>の登録は、参照渡しで行われる。</para>
        /// <para>・登録するクラスには<see cref="DataContractAttribute"/>属性を、メンバには<see cref="DataMemberAttribute"/>属性を付与しなければならない。</para>
        /// <example>
        /// <code>
        /// [DataContract]
        /// class Sample
        /// {
        ///		[DataMember]
        ///		int value1 = 1;
        ///		[DataMember]
        ///		double value2 = 2.0;
        ///	}
        /// </code>
        /// </example>
        /// </param>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/>がnull又は空です。</exception>
        /// <exception cref="ArgumentException"><paramref name="key"/>が登録済み。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/>がnullです。</exception>
        public void Register<T>(string key, T parameter) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new KeyNotFoundException(nameof(key));
            }

            if (registerTable.ContainsKey(key))
            {
                throw new ArgumentException("The key has been registered.", nameof(key));
            }

            ArgumentNullException.ThrowIfNull(parameter);

            registerTable.Add(key, parameter);
            typeTable.Add(key, typeof(T));

            return;
        }

        /// <summary>
        /// パラメータを削除します
        /// </summary>
        /// <remarks>指定されたキーが登録されていない場合、削除処理は実施されません。例外もthrowされません。</remarks>
        /// <param name="key">削除するパラメーターのキー</param>
        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            registerTable.Remove(key);
            typeTable.Remove(key);

            return;
        }

        /// <summary>
        /// システムファイルから読み込んだパラメータを参照します
        /// </summary>
        /// <remarks>事前に<see cref="Load(string)"/>でシステムファイルを読み込む必要があります。</remarks>
        /// <typeparam name="T">戻り値の型</typeparam>
        /// <param name="key">パラメーターと対になるキー</param>
        /// <exception cref="NullReferenceException">システムファイルが読み込まれていない。</exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/>が登録されていない。</exception>
        /// <exception cref="TypeAccessException">戻り値の型が登録されている型と一致しない。</exception>
        /// <returns>指定されたキーに関連付けられているパラメーター</returns>
        public T Refer<T>(string key) where T : class
        {
            if (referenceTable is null)
            {
                throw new NullReferenceException();
            }

            if (!referenceTable.TryGetValue(key, out object? value))
            {
                throw new KeyNotFoundException();
            }

            if (typeof(T) != typeTable[key])
            {
                throw new TypeAccessException();
            }

            return (T)value;
        }
        #endregion // Methods

        #region Classes
        /// <summary>
        /// XMLファイル生成用クラス
        /// </summary>
        [DataContract]
        internal class XmlFile
        {
            [DataMember]
            private Dictionary<string, object> parameterTable;

            public XmlFile(IDictionary<string, object> registerTable)
            {
                parameterTable = [];
                foreach (var parameter in registerTable)
                {
                    parameterTable.Add(parameter.Key, parameter.Value);
                }
            }

            public Dictionary<string, object> ReadParameter()
            {
                return parameterTable;
            }
        }
        #endregion // Classes
    }
}
