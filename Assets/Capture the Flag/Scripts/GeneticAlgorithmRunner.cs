using System.Collections.Generic;
using UnityEngine;

namespace Capture_the_Flag
{
    public class GeneticAlgorithmRunner : MonoBehaviour
    {
        [SerializeField] private CaptureTheFlagGame _game;
        [SerializeField] private GeneticAlgorithmParameters _parameters;

        
        private void Start()
        {
            var brains = CreateInitialGenerationBrains();
            
            _game.Initialize(_parameters);
            _game.Begin(brains);

            _game.OnDone += OnGameDone;
        }

        private void OnDestroy()
        {
            _game.OnDone -= OnGameDone;
        }


        private void OnGameDone()
        {
            var bestPlayers = _game.GetBestPlayers();
            var nextGenBrains = CreateNextGenerationBrains(bestPlayers);
            
            _game.ResetState();
            _game.Begin(nextGenBrains);
        }


        private List<CaptureTheFlagPlayerBrain> CreateInitialGenerationBrains()
        {
            var brains = new List<CaptureTheFlagPlayerBrain>(_parameters.populationCount);
            
            for (var i = 0; i < _parameters.populationCount; i++)
            {
                brains.Add(CaptureTheFlagPlayerBrain.New(_parameters.brainBatchSize));
            }

            return brains;
        }
        
        private List<CaptureTheFlagPlayerBrain> CreateNextGenerationBrains(CaptureTheFlagPlayer[] players)
        {
            var brains = new List<CaptureTheFlagPlayerBrain>(_parameters.populationCount);
            var newBrainCount = _parameters.populationCount - players.Length;
            for (var i = 0; i < newBrainCount; i++)
            {
                const int maxIter = 100;
                var a = players[Random.Range(0, players.Length)].GetBrain();
                var j = 0;
                CaptureTheFlagPlayerBrain b;
                do
                {
                    b = players[Random.Range(0, players.Length)].GetBrain();
                    j++;
                } while (b == a && j < maxIter);

                var newBrain = ReproduceBrain(a, b);
                newBrain.IncreaseSize(_parameters.brainBatchSize);
                brains.Add(newBrain);
            }
            foreach (var player in players)
            {
                var brain = player.GetBrain();
                brain.IncreaseSize(_parameters.brainBatchSize);
                brains.Add(brain);
            }

            foreach (var brain in brains)
            {
                brain.Mutate(_parameters.brainBatchSize, _parameters.mutationRate);
            }

            return brains;
        }

        private CaptureTheFlagPlayerBrain ReproduceBrain(CaptureTheFlagPlayerBrain a, CaptureTheFlagPlayerBrain b)
        {
            var brain = CaptureTheFlagPlayerBrain.New(0);
            
            for (var i = 0; i < a.GetSize(); i++)
            {
                brain.AddAction(Random.Range(0f, 1f) < 0.5f ? a.GetAction(i) : b.GetAction(i));
            }

            return brain;
        }
    }
}