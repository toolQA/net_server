using UnityEngine;
using System.IO;
using SimpleJSON;
using System.Text;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

public class Tools
{
    GameObject toolGo = null;

    public static Tools t(GameObject go)
    {
        Tools gow = new Tools();
        gow.toolGo = go;
        return gow;
    }

    #region gameObject
    /// <summary>
    /// 重置 预制件名字 移除（Clone）
    /// </summary>
    /// <param name="go"></param>
    public static void NameReset(GameObject go)
    {
        int fpos = go.name.IndexOf("(");
        if (fpos >= 0)
        {
            go.name = go.name.Substring(0, fpos);
        }
    }

    /// <summary>
    /// 返回物体内名字为 “” 的gameobject
    /// </summary>
    static GameObject findGo = null;
    public static GameObject GetNameFindGameObject(GameObject go, string name)
    {
        findGo = null;
        GetFindGameObjectName(go, name);

        if (findGo != null)
        {
            return findGo;
        }
        return findGo;
    }

    static void GetFindGameObjectName(GameObject go, string name)
    {
        bool find = false;
        for (int i = 0; i < go.transform.childCount; i++)
        {
            if (go.transform.GetChild(i).name == name)
            {
                find = true;
                findGo = go.transform.GetChild(i).gameObject;
                return;
            }
        }
        if (!find)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                if (go.transform.GetChild(i).childCount > 0)
                {
                    GetFindGameObjectName(go.transform.GetChild(i).gameObject, name);
                }
            }
        }
    }

    public static void DestroyAllChild(Transform parent)
    {
        //Debug.Log(parent.name);
        while (parent.childCount > 0)
        {
            GameObject.DestroyImmediate(parent.GetChild(0).gameObject);
        }
    }

    #endregion

    #region create gameObject
    /// <summary>
    /// 读取创建预制
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static GameObject CreateGameObject(string path)
    {
        if (path == null || path == "") return null;
        GameObject obj = null;
        try
        {
            obj = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            NameReset(obj);
        }
        catch
        {
            Debug.LogError("!!!! path = " + path);
        }
        return obj;
    }

    /// <summary>
    /// 读取创建预制 并设置父类
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject CreateGameObjectTr(string path, Transform parent)
    {
        if (path == null || path == "") return null;
        GameObject go = null;
        try
        {
            go = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            NameReset(go);
        }
        catch
        {
            Debug.LogError("!!!! path = " + path);
        }
        try
        {
            if (!parent.gameObject.activeSelf)
            {
                parent.gameObject.SetActive(true);
            }
            go.transform.SetParent(parent.transform, false);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
        }
        catch
        {
            Debug.LogError("!!!! GO  Tr = Null");
        }
        return go;
    }

    /// <summary>
    /// 读取创建预制 并设置父类
    /// </summary>
    /// <param name="perfab"></param>
    /// <param name="parent"></param>
    /// <param name="isSetUIZero"></param>
    /// <returns></returns>
    public static GameObject CreateGameObjectTr(GameObject perfab, Transform parent, bool isSetUIZero = false)
    {
        GameObject go = null;
        try
        {
            go = GameObject.Instantiate(perfab);
            NameReset(go);
        }
        catch
        {
            Debug.LogError("!!!! perfab is null.");
        }
        try
        {
            go.SetActive(true);
            go.transform.SetParent(parent, false);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            if (isSetUIZero)
            {
                Set_UIAnchorsZero(go);
            }
        }
        catch
        {
            Debug.Log("!!!! GO  Tr = Null");
        }
        return go;
    }
    #endregion

    #region image
    public static void SetImage(GameObject go, Texture2D tex)
    {
        try
        {
            if (go != null && tex != null)
            {
                go.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }
        catch
        {
            Debug.Log("[Null] go");
        }
    }

    public static void SetRawImage(GameObject go, Texture2D tex)
    {
        try
        {
            if (go != null && tex != null)
            {
                go.GetComponent<RawImage>().texture = tex;
                //go.GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width, tex.height);
            }
        }
        catch
        {
            Debug.Log("[Null] go");
        }
    }

    /// <summary>
    /// 修改替换 图片
    /// </summary>
    /// <param name="go"></param>
    /// <param name="path"></param>
    /// <param name="img_path"></param>
    public static void SetGoImage(GameObject go, string path, string img_path)
    {
        try
        {
            Texture2D t = Resources.Load(img_path, typeof(Texture2D)) as Texture2D;
            Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));

            if (s != null && go != null)
            {
                if (path == "")
                {
                    go.GetComponent<Image>().sprite = s;
                }
                else
                {
                    go.transform.Find(path).GetComponent<Image>().sprite = s;
                }
            }
        }
        catch
        {
            Debug.LogError("!!!替换图片出错 == " + go.name + " || " + path + " ||" + img_path);
        }
    }

    /// <summary>
    /// 修改替换 图片
    /// </summary>
    /// <param name="go"></param>
    /// <param name="path"></param>
    /// <param name="img_path"></param>
    public static void ChangeGoImage(GameObject go, string path, string img_path)
    {
        try
        {
            Sprite s = Resources.Load(img_path, typeof(Sprite)) as Sprite;

            if (s != null && go != null)
            {
                if (path == "")
                {
                    go.GetComponent<Image>().sprite = s;
                }
                else
                {
                    go.transform.Find(path).GetComponent<Image>().sprite = s;
                }
            }
        }
        catch
        {
            Debug.LogError("!!!替换图片出错 == " + go.name + " || " + path + " ||" + img_path);
        }
    }

    /// <summary>
    /// 修改替换 图片  并设置图片原尺寸
    /// </summary>
    /// <param name="go"></param>
    /// <param name="path"></param>
    /// <param name="img_path"></param>
    public static void SetGoImageResetSize(GameObject go, string path, string img_path)
    {
        try
        {
            Texture2D t = Resources.Load(img_path, typeof(Texture2D)) as Texture2D;

            Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));

            if (s != null && go != null)
            {
                if (path == "")
                {
                    go.GetComponent<Image>().sprite = s;
                    go.GetComponent<RectTransform>().sizeDelta = new Vector2(t.width, t.height);
                }
                else
                {
                    go.transform.Find(path).GetComponent<Image>().sprite = s;
                    go.transform.Find(path).GetComponent<RectTransform>().sizeDelta = new Vector2(t.width, t.height);
                }
            }
        }
        catch
        {
            Debug.LogError("!!!替换图片出错 == " + go.name + " || " + path + " ||" + img_path);
        }
    }
    #endregion

    #region Text
    /// <summary>
    /// UGUI返回 text
    /// </summary>
    /// <param name="go"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Text GetUGUI_Text(GameObject go, string path)
    {
        if (go == null || path == null) { return null; }
        Text text;
        if (path == "")
        {
            text = go.GetComponent<Text>();
        }
        else
        {
            text = go.transform.Find(path).GetComponent<Text>();
        }

        return text;
    }

    /// <summary>
    /// 设置文本的内容
    /// </summary>
    /// <param name="go"></param>
    /// <param name="path"></param>
    /// <param name="info"></param>
    public static void SetText_Info(GameObject go, string path, string info)
    {
        if (go == null) { return; }
        Text text = null;
        if (string.IsNullOrEmpty(info))
        {
            text = go.GetComponent<Text>();
        }
        else
        {
            text = go.transform.Find(path).GetComponent<Text>();
        }

        text.text = info;
    }

    public static void SetText_InfoForBestFit(GameObject go, string path, string info)
    {
        if (go == null) { return; }
        Text text;
        if (path == "")
        {
            text = go.GetComponent<Text>();
        }
        else
        {
            text = go.transform.Find(path).GetComponent<Text>();
        }

        if (info.Length < 4)
        {
            text.resizeTextForBestFit = false;
        }
        else
        {
            text.resizeTextForBestFit = true;
        }

        text.text = info;
    }

    public static void SetText_InfoForExpand(GameObject go, string path, string info)
    {
        if (go == null || path == null)
        {
            return;
        }
        Text text = null;
        if (String.IsNullOrEmpty(info))
        {
            text = go.GetComponent<Text>();
        }
        else
        {
            text = go.transform.Find(path).GetComponent<Text>();
        }

        text.text += "\r\n" + info;
    }
    #endregion

    #region button
    /// <summary>
    /// 设置按键 响应
    /// </summary>
    /// <param name="go"></param>
    /// <param name="call"></param>
    /// <param name="path"></param>
    public static void SetButton(GameObject go, ToolDelegate.Void call, string path = "")
    {
        if (!string.IsNullOrEmpty(path))
        {
            go = go.transform.Find(path).gameObject;
        }

        Button btn = go.GetComponent<Button>();

        if (btn == null)
        {
            btn = go.AddComponent<Button>();
        }

        btn.onClick.RemoveAllListeners();
#if UNITY_5_6
        btn.onClick.AddListener(delegate () { call(); });
#elif UNITY_2018_4
        btn.onClick.AddListener(delegate () { call?.Invoke(); });
#endif
    }

    /// <summary>
    /// 设置按键 响应  带参
    /// </summary>
    /// <param name="go"></param>
    /// <param name="call"></param>
    /// <param name="path"></param>
    public static void SetButton(GameObject go, ToolDelegate.String call, string info, string path = "")
    {
        if (!string.IsNullOrEmpty(path))
        {
            go = go.transform.Find(path).gameObject;
        }

        Button btn = go.GetComponent<Button>();

        if (btn == null)
        {
            btn = go.AddComponent<Button>();
        }

        btn.onClick.RemoveAllListeners();
#if UNITY_5_6
        btn.onClick.AddListener(delegate () { call(info); });
#elif UNITY_2018_4
        btn.onClick.AddListener(delegate () { call?.Invoke(info); });
#endif
    }

    /// <summary>
    /// 设置按键 响应  带参
    /// </summary>
    /// <param name="go"></param>
    /// <param name="call"></param>
    /// <param name="path"></param>
    public static void SetButton(GameObject go, ToolDelegate.Int call, int info, string path = "")
    {
        if (!string.IsNullOrEmpty(path))
        {
            go = go.transform.Find(path).gameObject;
        }

        Button btn = go.GetComponent<Button>();

        if (btn == null)
        {
            btn = go.AddComponent<Button>();
        }

        btn.onClick.RemoveAllListeners();
#if UNITY_5_6
        btn.onClick.AddListener(delegate () { call(info); });
#elif UNITY_2018_4
        btn.onClick.AddListener(delegate () { call?.Invoke(info); });
#endif
    }

    /// <summary>
    /// 设置按键 响应  带参
    /// </summary>
    /// <param name="go"></param>
    /// <param name="call"></param>
    /// <param name="path"></param>
    public static void SetButton(GameObject go, ToolDelegate.Json call, JSONNode json, string path = "")
    {
        if (!string.IsNullOrEmpty(path))
        {
            go = go.transform.Find(path).gameObject;
        }

        Button btn = go.GetComponent<Button>();

        if (btn == null)
        {
            btn = go.AddComponent<Button>();
        }

        btn.onClick.RemoveAllListeners();
#if UNITY_5_6
        btn.onClick.AddListener(delegate () { call(json); });
#elif UNITY_2018_4
        btn.onClick.AddListener(delegate () { call?.Invoke(json); });
#endif
    }

    public static void ClearButtonCallback(GameObject go, string path)
    {
        if (go!= null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                go.transform.Find(path).GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }
#endregion

    #region color
        /// <summary>
        /// 颜色字符（0xffffffff）转换 color
        /// </summary>
        /// <param name="colorstring"></param>
        /// <returns></returns>
        public static Color ColorFromString(string colorstring)
        {

            int r = VFromChar(colorstring[0]) * 16 + VFromChar(colorstring[1]);
            int g = VFromChar(colorstring[2]) * 16 + VFromChar(colorstring[3]);
            int b = VFromChar(colorstring[4]) * 16 + VFromChar(colorstring[5]);
            int a = VFromChar(colorstring[6]) * 16 + VFromChar(colorstring[7]);
            return new UnityEngine.Color(r * 1f / 255, g * 1f / 255, b * 1f / 255, a * 1f / 255);
        }
        static int VFromChar(int c)
        {
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }
            else if (c >= 'A' && c <= 'F')
            {
                return c - 'A' + 10;
            }
            else
            {
                return c - 'a' + 10;
            }
        }
    #endregion

    #region position
        /// <summary>
        /// 3D物体在2D屏幕上的位置
        /// </summary>
        /// <param name="gobj3d"></param>
        /// <param name="camer3d"></param>
        /// <param name="camera2d"></param>
        /// <param name="z"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector3 GetUIPosBy3DGameObj(GameObject gobj3d,
                                       Camera camer3d, Camera camera2d, float z, float y)
        {
            Vector3 v1 = camer3d.WorldToViewportPoint(new Vector3(gobj3d.transform.position.x, y, gobj3d.transform.position.z));
            Vector3 v2 = camera2d.ViewportToWorldPoint(v1);
            v2.z = z;
            return v2;
        }

        /// <summary>
        /// 设置2d物体 到 3D物体在屏幕上的位置
        /// </summary>
        /// <param name="gobj2d"></param>
        /// <param name="gobj3d"></param>
        /// <param name="camer3d"></param>
        /// <param name="camera2d"></param>
        /// <param name="z"></param>
        /// <param name="offset"></param>
        public static void SetUIPosBy3DGameObj(GameObject gobj2d, GameObject gobj3d,
                                       Camera camer3d, Camera camera2d, float z, Vector3 offset)
        {
            Vector3 v1 = camer3d.WorldToViewportPoint(gobj3d.transform.position);
            Vector3 v2 = camera2d.ViewportToWorldPoint(v1);
            v2.z = z;
            gobj2d.transform.position = v2 + offset;
        }
    #endregion

    #region 外部 写文件
        /// <summary>
        /// 写 二进制文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        public static void WriteBytes(string path, byte[] bytes)
        {
            JudgeCreateFilePath(path);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 写 txt文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        public static void WriteTxt(string path, string text)
        {
            JudgeCreateFilePath(path);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(text.ToString());
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 检查文件目录是否存在  不在就创建
        /// </summary>
        /// <param name="path"></param>
        public static void JudgeCreateFilePath(string path)
        {
            string[] temp = path.Split('/');
            int num = path.Length - temp[temp.Length - 1].Length - 1;
            path = path.Remove(num, path.Length - num);

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }
    #endregion

    #region 内部 读文件
        /// <summary>
        /// 内部 读取对象 Object
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static UnityEngine.Object LoadResources(string path)
        {
            UnityEngine.Object obj = Resources.Load(path);
            if (obj == null) return null;
            UnityEngine.Object go = UnityEngine.Object.Instantiate(obj);
            return path != null ? go : null;
        }

        /// <summary>
        /// 内部 读取 音效
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AudioClip LoadAudio(string path)
        {
            // AudioClip audio = new AudioClip();

            try
            {
                //     audio = (AudioClip)Resources.Load(path, typeof(AudioClip));
            }
            catch
            {
                Debug.LogError("!!!! audio  = Null  path =" + path);
                return null;
            }
            //   return audio;
            return null;
        }

        /// <summary>
        /// 内部 读取 Texture2D
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Texture2D LoadTexture2D(string path)
        {
            Texture2D tex = new Texture2D(1, 1);

            try
            {
                tex = (Texture2D)Resources.Load(path, typeof(Texture2D));
            }
            catch
            {
                Debug.LogError("!!!! Texture2D  = Null  path =" + path);
                return null;
            }
            return tex;
        }

        /// <summary>
        /// 内部 读文件 String
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string LoadGameString(string path)
        {
            string txt = ((TextAsset)Resources.Load(path)).text;
            return txt;
        }

        /// <summary>
        /// 内部 读文件 json
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static JSONNode LoadGameJson(string path)
        {
            string txt = ((TextAsset)Resources.Load(path)).text;
            JSONNode json = JSONClass.Parse(txt);
            return json;
        }
    #endregion

    #region 外部 读文件
        /// <summary>
        /// 外部 读txt文件并返回json
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static JSONNode LoadJSON(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                int len = (int)fs.Length;
                byte[] byData = new byte[len];
                fs.Read(byData, 0, len);
                string text = Encoding.UTF8.GetString(byData);
                return JSONClass.Parse(text);
            }
        }

        /// <summary>
        /// 外部  读取二进制文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] LoadBytes(string path)
        {
            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                int len = (int)file.Length;
                byte[] byData = new byte[len];
                file.Read(byData, 0, len);
                return byData;
            }
        }

        /// <summary>
        /// 外部 读txt文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string LoadString(string path)
        {
            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                int len = (int)file.Length;
                byte[] byData = new byte[len];
                file.Read(byData, 0, len);
                return Encoding.UTF8.GetString(byData);
            }
        }

        /// <summary>
        /// 外部 读byte文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] LoadWinBytes(string path)
        {
            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                int len = (int)file.Length;
                byte[] byData = new byte[len];
                file.Read(byData, 0, len);
                return byData;
            }
        }
    #endregion

    #region file
        /// <summary>
        /// 删除文件目录中的文件
        /// </summary>
        /// <param name="path"></param>
        public static void Remove(string path)
        {
            File.Delete(path);
        }
    #endregion

    #region string
        /// <summary>
        /// 每两个字符之间相隔一个空格
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string AddBlankSpace(string info)
        {
            string str = "";
            for (int strIndex = 0; strIndex < info.Length; strIndex++)
            {
                str += info[strIndex] + " ";
            }
            return str;
        }

        ////字符串转首字母缩写
        //public static string ChinesecharToPinyin(string selwords)
        //{
        //    string newwords = "";
        //    if (selwords != "")
        //    {
        //        //判断是汉字还是字母
        //        string pattern = @"^[A-Za-z]+$";
        //        Regex regex = new Regex(pattern);
        //        //字母模糊搜索
        //        if (!regex.IsMatch(selwords))
        //        {
        //            selwords = selwords.ToLower();
        //            for (int i = 0; i < selwords.Length; i++)
        //            {
        //                var res = NPinyin.Pinyin.GetPinyin(selwords[i]);
        //                //验证该汉字是否合法
        //                if (ChineseChar.IsValidChar(res[0]))
        //                {
        //                    ChineseChar CC = new ChineseChar(selwords[i]);
        //                    //将该汉字转化为拼音集合
        //                    newwords += CC.Pinyins[0].ToLower()[0];

        //                }
        //                else
        //                {
        //                    newwords += res[0];
        //                }
        //            }
        //        }
        //        else
        //        {
        //            newwords = selwords;
        //        }

        //    }

        //    return newwords;
        //}
        /// <summary>
        /// 字符串是否 有中文字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsChinese(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(text[i].ToString(), @"^[\u4e00-\u9fa5]+$"))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 字符串是否 有 特殊符号
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsSymbol(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (!char.IsLetter(text[i]) && !char.IsNumber(text[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 字符串长度（中文字为2个字符）
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int GetStringLength(string text)
        {
            int num = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(text[i].ToString(), @"^[\u4e00-\u9fa5]+$"))
                {
                    num++;
                }
            }

            return text.Length + num;
        }

        /// <summary>
        /// 中英字  是否超出长度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool IsStringLength(string text, int num)
        {
            if (text.Length > num) return true;

            int temp = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(text[i].ToString(), @"^[\u4e00-\u9fa5]+$"))
                {
                    temp++;
                }
            }
            Debug.LogError(text + "==" + temp + "===" + text.Length + "  " + temp);
            if (text.Length + temp > num)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 字符串 是否 纯数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumber(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!Char.IsNumber(str, i))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 特殊字符后加字符 保留原字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringSwitchAdd(string str, char char_1, string char_2)
        {
            if (str.Contains(char_1.ToString()))
            {
                string[] temp = str.Split(char_1);
                str = "";
                for (int i = 0; i < temp.Length; i++)
                {
                    str += temp[i] + char_1 + char_2;
                }
            }

            return str;
        }

        /// <summary>
        /// 是否 是正确 的邮箱地址
        /// </summary>
        /// <param name="str_email"></param>
        /// <returns></returns>
        public static bool IsEmail(string str_email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 解析时间戳 中文日期
        /// </summary>
        /// <param name="_time"></param>
        /// <returns></returns>
        public static string[] GetTimeStamp_ch(string _time)
        {
            long timeStamp = long.Parse(_time);
            System.DateTime dtStart = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            long lTime = timeStamp * 10000000;

            System.TimeSpan toNow = new System.TimeSpan(lTime);

            System.DateTime dtResult = dtStart.Add(toNow);
            string date = dtResult.ToShortDateString().ToString();
            string time = dtResult.ToString("HH:mm:ss");
            string[] date_arr = date.Split('/');
            string[] time_arr = time.Split(':');
            string secondarr = time_arr[2];
            char[] second = secondarr.ToCharArray();

            string[] result = new string[]{ date_arr[2] + "年" + date_arr[0] + "月" + date_arr[1] + "日",
                time_arr[0] + ":" +time_arr[1] + ":" + second[0] + second[1]};

            return result;
        }

        /// <summary>
        /// 解析时间戳 xxxx/xx/xx
        /// </summary>
        /// <param name="_time"></param>
        /// <returns></returns>
        public static string[] GetTimeStamp(string _time)
        {
            long timeStamp = long.Parse(_time);
            System.DateTime dtStart = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            long lTime = timeStamp * 10000000;

            System.TimeSpan toNow = new System.TimeSpan(lTime);

            System.DateTime dtResult = dtStart.Add(toNow);
            string date = dtResult.ToShortDateString().ToString();
            string time = dtResult.ToString("HH:mm:ss");
            string[] date_arr = date.Split('/');
            string[] time_arr = time.Split(':');
            string secondarr = time_arr[2];
            char[] second = secondarr.ToCharArray();

            string[] result = new string[]{ date_arr[2] + "/" + date_arr[0] + "/" + date_arr[1],
                time_arr[0] + ":" +time_arr[1] + ":" + second[0] + second[1]};

            return result;
        }

        /// <summary>
        /// 括号换行
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string IsBrackets(string str)
        {
            if (str.Contains("（"))//中文
            {
                str = str.Split('（')[0] + "\n（" + str.Split('（')[1];

            }
            else if (str.Contains("("))//英文
            {
                str = str.Split('(')[0] + "\n(" + str.Split('(')[1];
            }
            return str;
        }

        public static string SwitchCN_EN_Brackets(string str)
        {
            if (str.Contains("（"))
            {
                str = str.Replace("（", "(");
            }
            if (str.Contains("）"))
            {
                str = str.Replace("）", ")");
            }
            return str;
        }

        /// <summary>
        /// 换行
        /// </summary>
        /// <param name="str"></param>
        public static string ToNextLine(string str, ref int row)
        {
            if (str == "" || str == null || str == " ")
                return null;
            string info = null;
            if (str.Contains("；"))
            {
                int c = Regex.Matches(str, @"；").Count;
                for (int i = 0; i < c + 1; i++)
                {
                    string data = str.Split('；')[i] + "\n";
                    info = info + data;
                }
                str = info;
            }
            else if (str.Contains(";"))
            {
                int c = Regex.Matches(str, @":").Count;
                for (int i = 0; i < c + 1; i++)
                {
                    string data = str.Split(';')[i] + "\n";
                    info = info + data;
                }
                str = info;
            }
            if (str.Contains("\n"))
            {
                int c = Regex.Matches(str, "\n").Count;
                row += c;
            }
            return str;
        }
        /// <summary>
        /// 秒  转 小时 分 秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string FormatTime_H(int seconds)
        {
            int intH = seconds / 3600;
            string strH = intH < 10 ? "0" + intH.ToString() : intH.ToString();
            int intM = (seconds % 3600) / 60;
            string strM = intM < 10 ? "0" + intM.ToString() : intM.ToString();
            int intS = seconds % 3600 % 60;
            string strS = intS < 10 ? "0" + intS.ToString() : intS.ToString();
            return strH + ":" + strM + ":" + strS;
        }

        /// <summary>
        /// 秒  转 分 秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string FormatTime_M(int seconds)
        {
            int intM = seconds / 60;
            string strM = intM < 10 ? "0" + intM.ToString() : intM.ToString();
            int intS = seconds % 60;
            string strS = intS < 10 ? "0" + intS.ToString() : intS.ToString();
            return strM + ":" + strS;
        }

        /// <summary>
        /// 价格 设置单位从角转元
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetPrice(int num)
        {
            if (num % 10 > 0)
            {
                string buy = num.ToString();
                return buy.Insert(buy.Length - 1, ".");
            }

            return (num / 10).ToString();
        }

        /// <summary>
        /// 数字转 千分号 字符
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string Logic_NumToString(int num)
        {
            if (num == 0)
            {
                return "0";
            }
            else
            {
                return num.ToString("#,###");
            }
        }
        public static string Logic_NumToString(float num)
        {
            if (num == 0)
            {
                return "0";
            }
            else
            {
                return num.ToString("#,###");
            }
        }
    #endregion

    #region texture2D
        /// <summary>
        /// 纹理缩放
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <returns></returns>
        public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }

            result.Apply();
            return result;
        }
    #endregion

    #region rect
        /// <summary>
        /// 判断点是否在矩形
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool JudgePointIsRect(Vector3 vec, Rect rect)
        {
            if (vec.x < rect.x || vec.x > rect.x + rect.width)
            {
                return true;
            }
            if (vec.y < rect.y || vec.y > rect.y + rect.height)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断点是否在矩形
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool IsPointInRect(Vector3 vec, Rect rect)
        {
            if (vec.x > rect.x && vec.x < rect.x + rect.width)
            {
                if (vec.y > rect.y && vec.y < rect.y + rect.height)
                {
                    return true;
                }
            }
            return false;
        }
    #endregion

    #region int
        /// <summary>
        /// 返回比例   带小数点后一位
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <returns></returns>
        public static float GetZB_f1(int num1, int num2)
        {
            float zb = float.Parse(Math.Round((float)num1 / (float)num2 * 100).ToString("f1"));
            if (num1 > 0 && num2 > 0)
            {
                if (zb == 0)
                {
                    zb = 0.1f;
                }
                else if (zb == 100)
                {
                    zb = 99;
                }
            }
            return zb;
        }

        /// <summary>
        /// 返回比例  int
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <returns></returns>
        public static int GetZB_int(int num1, int num2)
        {
            int zb = (int)Math.Round((float)num1 / (float)num2 * 100);
            if (num1 > 0 && num2 > 0)
            {
                if (zb == 0)
                {
                    zb = 1;
                }
            }
            return zb;
        }
    #endregion

    #region rectTransform
        /// <summary>
        /// 自适应UI  重置对齐方式
        /// </summary>
        /// <param name="go"></param>
        public static void Set_UIAnchorsZero(GameObject go)
        {
            go.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            go.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        }
    #endregion

    //工具
    public Com Find<Com>(string childName) where Com : Component
    {
        if (toolGo.name == childName)
        {
            return toolGo.GetComponent<Com>();
        }
        Com[] childs = toolGo.GetComponentsInChildren<Com>(true);
        foreach (Com child in childs)
        {
            //Debug.Log("name is " + child.gameObject.name );
            if (child.gameObject.name == childName)
            {
                return child;
            }
        }
        return null;
    }

    //#region UI动画
    //    //自上而下运动
    //    public static void DTud(GameObject go, float top, float down)
    //    {
    //        float target = go.transform.position.y;
    //        go.transform.localPosition += Vector3.up * top * 100;
    //        Sequence mySequence = DOTween.Sequence();
    //        mySequence.Append(go.transform.DOMoveY(target + down, 0.5f));
    //        mySequence.Append(go.transform.DOMoveY(target, 0.3f));
    //    }
    //    //所有子类依次自上而下运动
    //    public static void DTCud(GameObject go, float top, float down, float time1, float time2)
    //    {
    //        float target;
    //        GameObject child;
    //        Sequence mySequence = DOTween.Sequence();
    //        for (int i = 0; i < go.transform.childCount; i++)
    //        {
    //            child = go.transform.GetChild(i).gameObject;
    //            target = child.transform.position.y;
    //            child.transform.localPosition += Vector3.up * top * 100;
    //            mySequence.Append(child.transform.DOMoveY(target + down, time1));
    //            mySequence.Append(child.transform.DOMoveY(target, time2));
    //        }
    //    }
    //    //从右到左运动
    //    public static void DTrl(GameObject go, float right, float left, float time1, float time2)
    //    {
    //        float target = go.transform.position.x;
    //        go.transform.localPosition += Vector3.right * right * 100;
    //        Sequence mySequence = DOTween.Sequence();
    //        mySequence.Append(go.transform.DOMoveX(target + left, time1));
    //        mySequence.Append(go.transform.DOMoveX(target, time2));
    //    }
    //    //所有子类依次从左到右运动
    //    public static void DTCrl(GameObject go, float right, float left, float time1, float time2)
    //    {
    //        float target;
    //        GameObject child;
    //        Sequence mySequence = DOTween.Sequence();
    //        for (int i = 0; i < go.transform.childCount; i++)
    //        {
    //            child = go.transform.GetChild(i).gameObject;
    //            target = child.transform.position.x;
    //            child.transform.localPosition += Vector3.right * right * 100;
    //            mySequence.Append(child.transform.DOMoveX(target + left, time1));
    //            mySequence.Append(child.transform.DOMoveX(target, time2));
    //        }
    //    }
    //    //翻转动画
    //    public static void DTfz(GameObject go, float time)
    //    {
    //        GameObject child;

    //        for (int i = 0; i < go.transform.childCount; i++)
    //        {
    //            //      Sequence mySequence = DOTween.Sequence();
    //            child = go.transform.GetChild(i).gameObject;
    //            child.transform.localScale = new Vector3(0, 0, 0);
    //            //   mySequence.Append(child.transform.DOScale(new Vector3(1, 1, 1), time).SetEase(Ease.OutElastic));
    //            child.transform.DOScale(new Vector3(1, 1, 1), time).SetEase(Ease.OutBounce);
    //        }
    //    }
    //    //放大动画
    //    public static void DTfd(GameObject go, float time)
    //    {
    //        GameObject child;
    //        for (int i = 0; i < go.transform.childCount; i++)
    //        {
    //            child = go.transform.GetChild(i).gameObject;
    //            child.transform.localScale = new Vector3(0, 0, 0);
    //            child.transform.DOScale(new Vector3(1, 1, 1), time).SetEase(Ease.OutBounce);
    //        }
    //    }
    //#endregion

    #region screen
        private static Vector2 portraitV2 = new Vector2(1080, 1920);
        private static Vector2 LandscapeV2 = new Vector2(1920, 1080);

        /// <summary>
        /// 屏幕翻转 竖屏
        /// </summary>
        public static void FlipScreenPortrait()
        {
            CanvasScaler cs = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
            cs.matchWidthOrHeight = 0;
            cs.referenceResolution = portraitV2;
            Screen.orientation = ScreenOrientation.Portrait;
        }

        /// <summary>
        /// 屏幕翻转 横屏
        /// </summary>
        public static void FlipScreenLandScape()
        {
            CanvasScaler cs = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
            cs.matchWidthOrHeight = 1;
            cs.referenceResolution = LandscapeV2;
            Screen.orientation = ScreenOrientation.Landscape;
        }

        public static void SetFullScreen(bool isFull)
        {
            Screen.fullScreen = isFull;
        }

        public static void IsFullScreen()
        {
    #if UNITY_2018_4
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    #endif
            }
    #endregion

    #region 适配
        public static void AdapteIPhoneX(Transform t)
        {
            bool IsIphoneXDevice = false;
            string modelStr = SystemInfo.deviceModel;
    #if UNITY_IOS
        // iPhoneX:"iPhone10,3","iPhone10,6"  iPhoneXR:"iPhone11,8"  iPhoneXS:"iPhone11,2"  iPhoneXS Max:"iPhone11,6"
        IsIphoneXDevice = modelStr.Equals("iPhone10,3") || modelStr.Equals("iPhone10,6") || modelStr.Equals("iPhone11,8") || modelStr.Equals("iPhone11,2") || modelStr.Equals("iPhone11,6");
    #endif

            if (IsIphoneXDevice)
            {
                t.GetComponent<RectTransform>().sizeDelta = new Vector2(0, -138);
            }
        }
    #endregion

    #region 获取本机ip, mac地址

    /// <summary>
    /// 获取本机MAC 地址
    /// </summary>
    /// <returns></returns>
    public static string GetMacAddress()
    {
        string physicalAddress = "";

        NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface adaper in nice)
        {

            if (adaper.Description == "en0")
            {
                physicalAddress = adaper.GetPhysicalAddress().ToString();
                break;
            }
            else
            {
                physicalAddress = adaper.GetPhysicalAddress().ToString();

                if (physicalAddress != "")
                {
                    break;
                };
            }
        }
        Debug.Log(physicalAddress);
        return physicalAddress;
    }

    /// <summary>
    /// 获取本机局域网内的ip地址组
    /// </summary>
    /// <param name="isGetIPV4">true获取ipv4，false为获取ipv6</param>
    /// <returns></returns>
    public static List<string> GetLocalIP(bool isGetIPV4)
    {
        IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());   //Dns.GetHostName()获取本机名Dns.GetHostAddresses()根据本机名获取ip地址组
        List<string> ipv4_s = new List<string>();
        List<string> ipv6_s = new List<string>();
        foreach (IPAddress ip in ips)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ipv4_s.Add(ip.ToString());
            }
            else if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                ipv6_s.Add(ip.ToString());
            }
        }

        if (isGetIPV4)
        {
            return ipv4_s;
        }
        else
        {
            return ipv6_s;
        }
    }


    #endregion


    #region zdt util
    
    /// <summary>
    /// 在已有内容的文件里添加新的内容
    /// </summary>
    /// <param name="path"></param>
    /// <param name="add_string"></param>
    /// <param name="type"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static bool WriteToFile(string path, string add_string, int offset)
    {
        if (!File.Exists(path))
        {
            Tools.WriteTxt(path, add_string);
        }
        else
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                int len = fs.ReadByte();

                byte[] readData = new byte[len];

                fs.Read(readData, 0, len);

                string last_str = Encoding.UTF8.GetString(readData, len - 1, 1);

                string write_str = last_str;

                byte[] writeData = Encoding.UTF8.GetBytes(write_str);

                fs.Write(writeData, offset,0 );
                fs.Flush();
            }
        }

        return true;
    }

    /// <summary>
    /// 判断是否为正确格式的ipv4地址
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public static bool IsCorrectIPV4(string ip)
    {
        ip = ip.Trim();

        if (!string.IsNullOrEmpty(ip))
        {
            string[] ip_s = ip.Split('.');

            if (ip_s.Length == 4)
            {
                for (int i = 0; i < ip_s.Length; i++)
                {
                    int ret = -1;

                    if (string.IsNullOrEmpty(ip_s[i]))
                    {
                        return false;
                    }

                    if (int.TryParse(ip_s[i], out ret))
                    {
                        if (ret < 0 || ret > 255)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        return false;
    }
    #endregion
}

public class ToolDelegate
{
    public delegate void Json(JSONNode json);
    public delegate void String(string txt);
    public delegate void Tex2D(Texture2D tex);
    public delegate void Go(GameObject go);
    public delegate void Void();
    public delegate void Int(int index);
    public delegate void Bool(bool info);
    public delegate bool GetBool();
    public delegate void Bytes(byte[] bytes);

    public delegate void Tex2D_Go(GameObject go, Texture2D tex);
}

public enum InsertStringToLocalFileType
{
    prev = 0,
    middle = 1,
    last = 2
}