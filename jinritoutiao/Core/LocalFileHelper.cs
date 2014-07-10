/* ==============================================================================
 * 类名称：LocalFileHelper
 * 类描述：
 * 创建人：better.chaner
 * 创建时间：2013/2/18 13:50:03
 * 修改人：
 * 修改时间：
 * 修改备注：
 * @version 1.0
 * ==============================================================================*/

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace jinritoutiao.Core
{
    /// <summary>
    /// LocalFileHelper
    /// </summary>
    public class LocalFileHelper
    {
        /// <summary>
        /// 存储数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="fileName">文件名称</param>
        /// <param name="data">数据</param>
        /// <returns>无</returns>
        public async static Task Save<T>(string fileName, T data)
        {
            //取得当前程序存放数据的目录  
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            //创建文件，如果文件存在就覆盖  
            StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (Stream newFileStream = await file.OpenStreamForWriteAsync())
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(T));
                ser.WriteObject(newFileStream, data);
                newFileStream.Dispose();
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="fileName">文件名称</param>
        /// <returns>数据</returns>
        public async static Task<T> Read<T>(string fileName)
        {
            T t = default(T);
            try
            {
                StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync(fileName);
                if (file == null)
                    return t;
                Stream newFileStream = await file.OpenStreamForReadAsync();

                DataContractSerializer ser = new DataContractSerializer(typeof(T));
                t = (T)ser.ReadObject(newFileStream);
                newFileStream.Dispose();
                return t;
            }
            catch (Exception)
            {
                return t;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns>成功true/失败false</returns>
        public async static Task<bool> Delete(string fileName)
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync(fileName);
            if (file != null)
            {
                try
                {
                    await file.DeleteAsync();
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
