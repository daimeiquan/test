﻿using System;

namespace FastDFS.Client.Common
{
    public class FDFSRequest
    {
        #region 公共属性

        public FDFSHeader Header { get; set; }

        public byte[] Body { get; set; }

        public Connection Connection { get; set; }

        #endregion

        #region 公共方法

        public byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 公共虚方法

        public virtual FDFSRequest GetRequest(params object[] paramList)
        {
            throw new NotImplementedException();
        }

        public virtual byte[] GetResponse()
        {
            if (Connection == null)
            {
                Connection = ConnectionManager.GetTrackerConnection();
            }

            try
            {
                //打开
                Connection.OpenConnection();

                var stream = Connection.GetStream();
                var headerBuffer = Header.ToByte();

                stream.Write(headerBuffer, 0, headerBuffer.Length);
                stream.Write(Body, 0, Body.Length);

                var header = new FDFSHeader(stream);
                if (header.Status != 0)
                    throw new FDFSException(string.Format("Get Response Error,Error Code:{0}", header.Status));



                var body = new byte[header.Length];
                //if (header.Length != 0) stream.Read(body, 0, (int)header.Length);

                int offset = 0, bytesread = 0;
                while (offset < header.Length)
                {
                    bytesread = stream.Read(body, offset, (int)header.Length - offset);
                    if (bytesread == 0)
                        throw new Exception("网络异常断开，数据读取不完整。");
                    else
                        offset += bytesread;
                }
                return body;  

                //return body;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //关闭
                //Connection.Close();
                Connection.ReleaseConnection();
            }
        }

        #endregion
    }
}