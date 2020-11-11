using HalconDotNet;
using System;
using System.Threading.Tasks;

namespace BingLibrary.HVision
{
    public class CameraOperate
    {
        //相机句柄
        private HFramegrabber Framegrabber { set; get; }

        public HImage CurrentImage { set; get; } = new HImage();
        public bool Connected { set; get; }

        /// <summary>
        /// 打开相机
        /// </summary>
        /// <param name="cameraName">相机名字</param>
        /// <param name="CameraInterface">相机接口</param>
        /// <returns></returns>
        public bool OpenCamera(string cameraName, string CameraInterface)
        {
            try
            {
                //        "DirectShow", 1, 1, 0, 0, 0, 0, "default", 8, "rgb", 
                //-1, "false", "default", "[0] Integrated Camera", 0, -1, out hv_AcqHandle
                //            Framegrabber = new HFramegrabber(CameraInterface, 1, 1, 0, 0, 0, 0, "default", 8, "rgb",
                //-1, "false", "default", cameraName, 0, -1);
                Framegrabber = new HFramegrabber(CameraInterface, 0, 0, 0, 0, 0, 0, "default", -1, "default", -1, "false", "default", cameraName, 0, -1);

                Connected = true;
                //GrabImageaAsyncStart();
                //Framegrabber.SetFramegrabberParam("AcquisitionMode", "SingleFrame");
               

                return true;
            }
            catch (HalconException ex){ Connected = false; return false; }
        }

        /// <summary>
        /// 关闭当前相机
        /// </summary>
        /// <returns></returns>
        public bool CloseCamera()
        {
            try
            {
                Framegrabber.Dispose();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 拍摄一张图片
        /// </summary>
        /// <returns></returns>
        public bool GrabImage()
        {
            try
            {
                if (Connected)
                {
                    CurrentImage?.Dispose();
                    CurrentImage = Framegrabber.GrabImage();
                    return true;
                }
                else return false;
            }
            catch
            {
                Connected = false;
                return false;
            }
        }

        public void GrabImageVoid()
        {
            try
            {
                if (Connected)
                {
                    CurrentImage?.Dispose();
                    CurrentImage = Framegrabber.GrabImage();
                   
                }
          
            }
            catch
            {
                Connected = false;
              
            }
        }




        /// <summary>
        /// 保存当前图片
        /// </summary>
        /// <param name="imageType">bmp,png,tiff</param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public bool SaveImage(string imageType, string imagePath)
        {
            try
            {
                CurrentImage.WriteImage(imageType, 0, imagePath);
                return true;
            }
            catch (Exception ex){ return false; }
        }

        /// <summary>
        /// 读取一张图片
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public bool ReadImage(string imagePath)
        {
            try
            {
                CurrentImage?.Dispose();
                CurrentImage.ReadImage(imagePath);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <param name="exposeValue"></param>
        /// <returns></returns>
        public bool SetExpose(double exposeValue)
        {
           
            try
            {
                Framegrabber.SetFramegrabberParam("ExposureTimeAbs", exposeValue);
                return true;
            }
            catch {  }
            try
            {
                Framegrabber.SetFramegrabberParam("ExposureTime", exposeValue);
                return true;
            }
            catch { }

            return false;
        }
    }
}