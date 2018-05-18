using dk.nsi.fmk;
using dk.nsi.seal;
using dk.nsi.seal.Factories;
using dk.nsi.seal.Serializers;
using NUnit.Framework;

namespace SealTest.Model
{
	public class SerializationTest : AbstractTest
	{

		[Test]
		public void IdCardSerializeStringTest()
		{
			//Create factory
			SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

			//Create IdCard
			UserIdCard idCard = CreateMocesUserIdCard(factory);

			//Sign IdCard
			Assertion ass = idCard.Sign<Assertion>(factory.SignatureProvider);

			var idCardString = IdCardSerializer.SerializeIdCardToString<UserIdCard>(idCard);
			var newIdCard = IdCardSerializer.DeserializeIdCard<UserIdCard>(idCardString);

			Assertion.Equals(idCard, newIdCard);
		}

		[Test]
		public void IdCardSerializeStreamTest()
		{
			//Create factory
			SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

			//Create IdCard
			UserIdCard idCard = CreateMocesUserIdCard(factory);

			//Sign IdCard
			Assertion ass = idCard.Sign<Assertion>(factory.SignatureProvider);

			var idCardStream = IdCardSerializer.SerializeIdCardToStream<UserIdCard>(idCard);
			var newIdCard = IdCardSerializer.DeserializeIdCard<UserIdCard>(idCardStream);

			Assertion.Equals(idCard, newIdCard);
		}

	}
}
