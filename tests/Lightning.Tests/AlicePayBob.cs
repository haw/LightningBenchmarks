using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Common.CLightning;
using Lightning.Alice;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Lightning.Tests
{
	public class AlicePayBob
	{
		Tester Tester;
		ActorTester<AliceRunner, AliceStartup> Alice;
		ActorTester<AliceRunner, AliceStartup> Bob;

		[GlobalSetup]
		public void Setup()
		{
			Tester = Tester.Create();
			Alice = Tester.CreateActor<Lightning.Alice.AliceRunner, Lightning.Alice.AliceStartup>("Alice");
			Bob = Tester.CreateActor<Lightning.Alice.AliceRunner, Lightning.Alice.AliceStartup>("Bob");
			Tester.Start();
			Tester.CreateChannel(Alice, Bob).GetAwaiter().GetResult();
		}


		[GlobalCleanup]
		public void Cleanup()
		{
			Tester.Dispose();
		}

		[Benchmark]
		public async Task RunAlicePayBob()
		{
			var invoice = await Bob.Runner.RPC.CreateInvoice(LightMoney.Satoshis(100));
			await Alice.Runner.RPC.SendAsync(invoice.BOLT11);
		}

		[Benchmark]
		public async Task RunCreateInvoice()
		{
			await Bob.Runner.RPC.CreateInvoice(LightMoney.Satoshis(100));
		}
	}
}
