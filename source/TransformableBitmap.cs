using System.Drawing;
using System.Drawing.Imaging;

namespace ImageGenerator
{
	public struct TransformableBitmap
	{
		public Bitmap Bitmap;
		public PointF Scale;
		public Point Origin;
		public float RotationAngle;

		public Rectangle Bounds
		{ 
			get
			{
				//TODO: Normal bounds calculation
				var widthWithoutRotation = Math.Max(Bitmap.Width * Scale.X, Bitmap.Height * Scale.Y);

				var width = (int)(widthWithoutRotation * 1.5);

				return new Rectangle(Origin.X - width / 2, Origin.Y - width / 2, width, width);
			}
		}

		public TransformableBitmap(Bitmap bitmap, Point origin, float rotationAngle, float scale)
		{
			Bitmap = bitmap;
			Origin = origin;
			RotationAngle = rotationAngle;
			Scale = new PointF(scale, scale);
		}

		public TransformableBitmap(Bitmap bitmap, Point origin, float rotationAngle, PointF scale)
		{
			Bitmap = bitmap;
			Origin = origin;
			RotationAngle = rotationAngle;
			Scale = scale;
		}

		public void DrawOnGraphic(Graphics gfx, Bitmap target)
		{
			var pixel = target.GetPixel(Math.Clamp(Origin.X, 0, target.Width - 1),
										Math.Clamp(Origin.Y, 0, target.Height - 1));

			float[][] ptsArray =
			{
				new float[] {pixel.R / 255f, 0, 0, 0, 0},
				new float[] {0, pixel.G / 255f, 0, 0, 0},
				new float[] {0, 0, pixel.B / 255f, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {0, 0, 0, 0, 1}
			};

			var clrMatrix = new ColorMatrix(ptsArray);
			var imgAttribs = new ImageAttributes();

			var tempOrigin = Origin - new Size(Bitmap.Width / 2, Bitmap.Height / 2);

			imgAttribs.SetColorMatrix(clrMatrix,
			ColorMatrixFlag.Default,
			ColorAdjustType.Default);
      //it just works
			gfx.TranslateTransform(Bitmap.Width / 2 + tempOrigin.X, Bitmap.Height / 2 + tempOrigin.Y);//1
			gfx.RotateTransform(RotationAngle);//2
			gfx.TranslateTransform(-(Bitmap.Width / 2 + tempOrigin.X), -(Bitmap.Height / 2 + tempOrigin.Y));
						
			gfx.DrawImage(
				Bitmap,
				new Rectangle(
					new Point((int)(Origin.X - Bitmap.Size.Width / 2 * Scale.X), (int)(Origin.Y - Bitmap.Size.Height / 2 * Scale.Y)),
					new Size((int)(Bitmap.Size.Width * Scale.X), (int)(Bitmap.Size.Height * Scale.Y))),
				0, 0, Bitmap.Size.Width, Bitmap.Size.Height, GraphicsUnit.Pixel, imgAttribs);
			
			gfx.TranslateTransform(Bitmap.Width / 2 + tempOrigin.X, Bitmap.Height / 2 + tempOrigin.Y);//1
			gfx.RotateTransform(-RotationAngle);
			gfx.TranslateTransform(-(Bitmap.Width / 2 + tempOrigin.X), -(Bitmap.Height / 2 + tempOrigin.Y));
		}
	}
}
