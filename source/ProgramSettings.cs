using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ImageGenerator
{
	public class ProgramSettings
	{
		public string PathToTarget { get; private set; } = "Images/target.png";
		public string PathToStart { get; private set; } = "Images/start.png";

		public int PretendersInIteration { get; private set; } = 100;
		public int SurvivingRateInChange { get; private set; } = 5;
		public int ChangesPerIteration { get; private set; } = 5;
		public int IterationsCount { get; private set; } = 10000;

		public bool SameScaleForBothAxis { get; private set; } = false;

		public float MinScale { get; private set; } = 0.1f;
		public float MaxScale { get; private set; } = 1;

		public float MinRotation { get; private set; } = 0.1f;
		public float MaxRotation { get; private set; } = 1;

		public int MaxOriginChange { get; private set; } = 40;
		public float MaxScaleChange { get; private set; } = 0.05f;
		public float MaxRotationChange { get; private set; } = 10;

		public static ProgramSettings FromFile(string filepath)
		{
			try
			{
				var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
				var result = deserializer.Deserialize<ProgramSettings>(File.ReadAllText(filepath));

				return result;
			}
			catch
			{
				//if that happend, file doesn't exists, so we need to create it
				var result = new ProgramSettings();
				result.SaveToFile(filepath);

				return result;
			}
		}


		public void SaveToFile(string filepath)
		{
			var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
			var stringResult = serializer.Serialize(this);

			File.WriteAllText(filepath, stringResult);
		}
	}
}
