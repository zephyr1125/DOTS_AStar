using System.Diagnostics;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Zephyr.DOTSAStar.Runtime.Component;
using Debug = UnityEngine.Debug;
using Random = Unity.Mathematics.Random;

namespace Zephyr.DOTSAStar.Tests
{
    public class PerformanceTest : TestBase
    {
        [Test]
        public void TestPerformance()
        {
            //map
            for (var i = 0; i < 100; i++)
            {
                var node = EntityManager.CreateEntity();
                EntityManager.AddComponentData(node, new TestNode
                {
                    Cost = 1, Id = i
                });
            }
            
            //request
            var random = new Random();
            random.InitState();
            for (var i = 0; i < 1000; i++)
            {
                var request = EntityManager.CreateEntity();
                EntityManager.AddComponentData(request, new PathFindingRequest
                {
                    StartId = 0, GoalId = random.NextInt(90,99)
                });
            }

            var system = World.GetOrCreateSystem<TestSystem>();
            system.resultECB =
                World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
            system.cleanECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            
            Stopwatch sw = Stopwatch.StartNew();
            
            system.Update();
            system.resultECB.Update();
            EntityManager.CompleteAllJobs();

            var resultQuery =
                EntityManager.CreateEntityQuery(
                    typeof(PathResult), typeof(PathRoute));
            var resultEntities = resultQuery.ToEntityArray(Allocator.TempJob);
            var results = resultQuery.ToComponentDataArray<PathResult>(Allocator.TempJob);
            
            Assert.AreEqual(1000, results.Length);
            foreach (var result in results)
            {
                Assert.AreEqual(true, result.Success, result.Log.ToString());
            }
            
            resultEntities.Dispose();
            results.Dispose();
            
            sw.Stop();
            Debug.Log(sw.ElapsedMilliseconds);
        }
    }
}