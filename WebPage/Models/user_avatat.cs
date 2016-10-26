using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Models
{
    public class user_avatat
    {        
       /// <summary>  
       /// 该方法是将姓名写入图像  
       /// </summary>  
       /// <param name="VNum">VNum是一个随机数</param>  
       public MemoryStream Create(string name)
       {           
           Bitmap Img = null;
           Graphics g = null;
           MemoryStream ms = null;

           //验证码字体集合  
           string[] fonts = { "Microsoft YaHei", "Comic Sans MS", "Arial", "宋体" };


           //定义图像的大小，生成图像的实例  
           Img = new Bitmap(80, 80);

           g = Graphics.FromImage(Img);//从Img对象生成新的Graphics对象    

           g.Clear(Color.LightGray);//背景 
 
           Font f = new System.Drawing.Font(fonts[0], 35, System.Drawing.FontStyle.Bold);//字体  
           Brush b = new System.Drawing.SolidBrush(Color.White);

           g.DrawString(name, f, b,new PointF(9,6));//绘制一个验证字符

           ms = new MemoryStream();//生成内存流对象  
           Img.Save(ms, ImageFormat.Jpeg);//将此图像以Png图像文件的格式保存到流中  

           //回收资源  
           g.Dispose();
           Img.Dispose();
           return ms;
       }
    }
}