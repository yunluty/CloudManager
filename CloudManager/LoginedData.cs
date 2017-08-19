using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudManager
{
    class LoginedData
    {
        string mPath;
        string[] mData;

        public LoginedData()
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dir += "\\CloudManager";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            mPath = dir + "\\login.dat";
            if (!File.Exists(mPath))
            {
                try
                {
                    File.Create(mPath);
                }
                catch (Exception)
                {
                    return;
                }
            }

            try
            {
                mData = File.ReadAllLines(mPath);
            }
            catch (Exception)
            {
                mData = null;
            }
        }      

        private int GetUserPosition(string user)
        {
            if (mData == null)
            {
                return -1;
            }

            for (int i = 0; i < mData.Length; i += 2)
            {
                if (string.Compare(mData[i], user) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public string GetPasswordByUser(string user)
        {
            if (mData == null)
            {
                return null;
            }

            int i = 0;

            for (i = 0; i < mData.Length; i += 2)
            {
                if (string.Compare(mData[i], user) == 0)
                {
                    return mData[i + 1];
                }
            }

            return null;
        }

        public bool SaveLoginedData(string user, string password)
        {
            int pos = GetUserPosition(user);
            if (pos < 0) //New user
            {
                string[] data;

                if (mData != null)
                {
                    data = new string[mData.Length + 2];
                    mData.CopyTo(data, 2);
                }
                else
                {
                    data = new string[2];
                }

                data[0] = user;
                data[1] = password;
                mData = data;

            }
            else
            {
                mData[pos] = user;
                mData[pos + 1] = password;
            }

            try
            { 
                File.WriteAllLines(mPath, mData);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void SaveLoginedData(string user)
        {
            SaveLoginedData(user, "");
        }
    }
}
