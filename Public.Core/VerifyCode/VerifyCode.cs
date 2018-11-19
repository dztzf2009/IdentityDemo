using System;
using System.DrawingCore;

namespace Public.Core.VerifyCode
{
    public class VerifyCode
    {
        private string _RandomCode = "";
        /// 生成随机码
        /// </summary>
        /// <param name="length">随机码个数</param>
        /// <returns></returns>
        public VerifyCode(int length)
        {
            int rand;
            char code;
            _RandomCode = String.Empty;

            //生成一定长度的验证码
            System.Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                rand = random.Next();

                if (rand % 3 == 0)
                {
                    code = (char)('A' + (char)(rand % 26));
                    if (code.Equals('O'))
                        code = 'A';
                }
                else
                {
                    code = (char)('0' + (char)(rand % 10));
                    if (code.Equals('0'))
                        code = '1';
                }

                _RandomCode += code.ToString();
            }
        }
        public string Code
        {
            get
            {
                return _RandomCode;
            }
        }
        /// <summary>
        /// 创建随机码图片
        /// </summary>
        /// <param name="randomcode">随机码</param>
        public string ImagePic
        {
            get
            {
                int randAngle = 45; //随机转动角度
                int mapwidth = (int)(_RandomCode.Length * 16);
                Bitmap map = new Bitmap(mapwidth, 22);//创建图片背景
                Graphics graph = Graphics.FromImage(map);
                graph.Clear(Color.AliceBlue);//清除画面，填充背景
                graph.DrawRectangle(new Pen(Color.Black, 0), 0, 0, map.Width - 1, map.Height - 1);//画一个边框
                                                                                                  //graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;//模式

                Random rand = new Random();

                //背景噪点生成
                Pen blackPen = new Pen(Color.LightGray, 0);
                for (int i = 0; i < 50; i++)
                {
                    int x = rand.Next(0, map.Width);
                    int y = rand.Next(0, map.Height);
                    graph.DrawRectangle(blackPen, x, y, 1, 1);
                }


                //验证码旋转，防止机器识别
                char[] chars = _RandomCode.ToCharArray();//拆散字符串成单字符数组

                //文字距中
                StringFormat format = new StringFormat(StringFormatFlags.NoClip);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                //定义颜色
                Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
                //定义字体
                string[] font = { "Verdana", "Microsoft Sans Serif", "Arial", "宋体" };
                int cindex = rand.Next(7);

                for (int i = 0; i < chars.Length; i++)
                {
                    int findex = rand.Next(4);

                    Font f = new Font(font[findex], 14, FontStyle.Bold);//字体样式(参数2为字体大小)
                    Brush b = new SolidBrush(c[cindex]);

                    Point dot = new Point(14, 14);
                    float angle = rand.Next(-randAngle, randAngle);//转动的度数

                    graph.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置
                    graph.RotateTransform(angle);
                    graph.DrawString(chars[i].ToString(), f, b, 1, 1, format);
                    graph.RotateTransform(-angle);//转回去
                    graph.TranslateTransform(-2, -dot.Y);//移动光标到指定位置，每个字符紧凑显示，避免被软件识别
                }
                //生成图片
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                map.Save(ms, System.DrawingCore.Imaging.ImageFormat.Png);
                var image = Convert.ToBase64String(ms.ToArray());
                graph.Dispose();
                map.Dispose();
                return string.Format("data:image/png;base64,{0}", image);
            }
        }
    }
}
