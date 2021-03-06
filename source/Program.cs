using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageGenerator
{
	public class Program
	{
		public static readonly Random Random = new();

		private static readonly ProgramSettings _settings = ProgramSettings.FromFile("settings.yml");
		private static readonly Bitmap _targetImage = new(_settings.PathToTarget);
		private static readonly byte[] _targetImageRgbValues = _targetImage.GetBytes();
		private static readonly List<Bitmap> _patterns = new();
		private static readonly Size ResultSize = _targetImage.Size;

		private static Bitmap _resultHandler = new(ResultSize.Width, ResultSize.Height);
		private static Graphics _resultGraphicHandler = Graphics.FromImage(_resultHandler);

		public static void Main()
		{
			SetUp();

			for (int currentIteration = 0; currentIteration < _settings.IterationsCount; currentIteration++)
			{
				List<TransformableBitmap> pretenders = new(_settings.PretendersInIteration);

				for (int i = 0; i < _patterns.Count; i++)
				{
					pretenders.Add(GetRandomTransformable(_patterns[i]));
				}

				for (int i = 0; pretenders.Count < _settings.PretendersInIteration; i++)
				{
					pretenders.Add(ChangeRandomly(pretenders[i % pretenders.Count]));
				}

				int drawBeforeIndex = 0;

				for (int currentChange = 0; currentChange < _settings.ChangesPerIteration; currentChange++)
				{
					Console.WriteLine($"iteration: {currentIteration}, change: {currentChange}");

					var surviversCount = _settings.PretendersInIteration / _settings.SurvivingRateInChange;

					pretenders.RemoveRange(surviversCount, Math.Max(pretenders.Count - surviversCount, 0));

					for (int i = 0; i < surviversCount; i++)
					{
						for (int ii = 1; ii < _settings.SurvivingRateInChange; ii++)
							pretenders.Add(ChangeRandomly(pretenders[i]));
					}

					drawBeforeIndex = OrderPretendersByRelevance(pretenders);
				}

				DrawMostRelevant(pretenders, drawBeforeIndex);

				if (Directory.Exists("Results") == false)
					Directory.CreateDirectory("Results");

				_resultHandler.Save($"Results/iteration_{currentIteration}.png");
			}

			_resultHandler.Save("Results/result.png");
		}

		private static void SetUp()
		{
			_resultHandler = new Bitmap(ResultSize.Width, ResultSize.Height);

			_resultGraphicHandler = Graphics.FromImage(_resultHandler);

			try
			{
				_resultHandler = new Bitmap(new Bitmap("Images/start.png"), ResultSize);
			}
			catch
			{
				_resultGraphicHandler.Clear(Color.White);
			}
			finally
			{
				_resultGraphicHandler.Dispose();

				_resultGraphicHandler = Graphics.FromImage(_resultHandler);
			}

			var patternsPaths = Directory.GetFiles("Images/Patterns/");

			foreach (var patternPath in patternsPaths)
			{
				_patterns.Add(new Bitmap(patternPath));
			}
		}

		//returns index first of first not relevant object
		private static int OrderPretendersByRelevance(List<TransformableBitmap> pretenders)
		{
			TransformableBitmap firstBad = pretenders[^1];

			int[] results = new int[pretenders.Count];

			for (int i = 0; i < pretenders.Count; i++)
			{
				var x = pretenders[i];

				var resultClone = new Bitmap(_resultHandler);
				var graphic = Graphics.FromImage(resultClone);

				x.DrawOnGraphic(graphic, _targetImage);

				var beforePretender = CompareImages(_resultHandler.GetBytes(), _targetImageRgbValues, x.Bounds, ResultSize);
				var afterPretender = CompareImages(resultClone.GetBytes(), _targetImageRgbValues, x.Bounds, ResultSize);

				graphic.Dispose();
				resultClone.Dispose();

				results[i] = afterPretender - beforePretender;
			}

			pretenders = pretenders.OrderBy(x => results[pretenders.IndexOf(x)]).ToList();

			Array.Sort(results);

			return Array.IndexOf(results, results.FirstOrDefault(x => x >= 0, 1));
		}

		private static void DrawMostRelevant(List<TransformableBitmap> pretenders, int lastIndexToCheck)
		{
			List<Rectangle> drawedRectangles = new() { pretenders[0].Bounds };

			pretenders[0].DrawOnGraphic(_resultGraphicHandler, _targetImage);
			
			for (int i = 1; i < lastIndexToCheck; i++)
			{
				var currentBounds = pretenders[i].Bounds;

				var intersectsOnce = drawedRectangles.Any(x => x.IntersectsWith(currentBounds));

				if (intersectsOnce == false)
				{
					drawedRectangles.Add(currentBounds);
					pretenders[i].DrawOnGraphic(_resultGraphicHandler, _targetImage);
				}
			}			
		}

		public static int GetDifferenceBetweenImages(byte[] rgbValues1, byte[] rgbValues2, Rectangle bounds, Size bitmapSize)
		{
			var startHorizontal = Math.Clamp(bounds.X, 0, bitmapSize.Width - 1);
			var startVertical = Math.Clamp(bounds.Y, 0, bitmapSize.Height - 1);

			var endHorizontal = Math.Clamp(bounds.X + bounds.Width, 0, bitmapSize.Width - 1);
			var endVertical = Math.Clamp(bounds.Y + bounds.Height, 0, bitmapSize.Height - 1);

			int difference = 0;

			for (int ii = startVertical; ii < endVertical; ii++)
			{
				for (int i = startHorizontal; i < endHorizontal; i++)
				{
					int pixelIndex = ((ii * bitmapSize.Width) + i) * 4;

					difference += Math.Abs(rgbValues1[pixelIndex] - rgbValues2[pixelIndex]) +
								  Math.Abs(rgbValues1[pixelIndex + 1] - rgbValues2[pixelIndex + 1]) +
								  Math.Abs(rgbValues1[pixelIndex + 2] - rgbValues2[pixelIndex + 2]);
				}
			}

			return difference;
		}

		public static TransformableBitmap GetRandomTransformable(Bitmap bitmap)
		{
			var origin = new Point(Random.Next(0, _targetImage.Width), Random.Next(0, _targetImage.Height));

			var scale = new PointF(Random.NextSingle() * (_settings.MaxScale - _settings.MinScale) + _settings.MinScale,
								   Random.NextSingle() * (_settings.MaxScale - _settings.MinScale) + _settings.MinScale);

			if (_settings.SameScaleForBothAxis)
				scale.Y = scale.X;

			var result = new TransformableBitmap(bitmap,
			                                     origin,
			                                     Random.NextSingle() * (_settings.MaxRotation - _settings.MinRotation) + _settings.MinRotation,
			                                     scale);

			return result;
		}

		public static TransformableBitmap ChangeRandomly(TransformableBitmap transformable)
		{
			var result = new TransformableBitmap(transformable.Bitmap, transformable.Origin, transformable.RotationAngle, transformable.Scale);

			result.Origin.X += (int)((Random.NextSingle() - 0.5) * 2 * _settings.MaxOriginChange);
			result.Origin.Y += (int)((Random.NextSingle() - 0.5) * 2 * _settings.MaxOriginChange);

			result.Scale.X = Math.Clamp((float)(result.Scale.X + (Random.NextSingle() - 0.5) * 2 * _settings.MaxScaleChange), _settings.MinScale, _settings.MaxScale);

			if (_settings.SameScaleForBothAxis)
				result.Scale.Y = result.Scale.X;
			else
				result.Scale.Y = Math.Clamp((float)(result.Scale.Y + (Random.NextSingle() - 0.5) * 2 * _settings.MaxScaleChange), _settings.MinScale, _settings.MaxScale);

			result.RotationAngle = Math.Clamp((float)(result.RotationAngle + (Random.NextSingle() - 0.5) * 2 * _settings.MaxRotationChange), _settings.MinRotation, _settings.MaxRotation);

			return result;
		}
	}
}
