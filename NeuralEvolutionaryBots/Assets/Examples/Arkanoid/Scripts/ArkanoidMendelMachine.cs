﻿using UnityEngine;
using System.Collections;
namespace EvolutionaryPerceptron.Examples.Arkanoid
{
    using EvolutionaryPerceptron.MendelMachine;
    public class ArkanoidMendelMachine : MendelMachine
    {
        public static ArkanoidMendelMachine current;

        [Header("ArkanoidData data")]
        public Scenario[] scenarios;
        public float lifeTime;

        [Header("Indicators")]
        [SerializeField]
        int index;

        Brain[] currentNeuralBot;
        GameObject[] currentBlocks;
        int count;
        protected override void Start()
        {
            if (!current)
                current = this;
            else
                Destroy(gameObject);

            base.Start();

            individualsPerGeneration = scenarios.Length;
            currentBlocks = new GameObject[individualsPerGeneration];
            currentNeuralBot = new Brain[individualsPerGeneration];
            for (int i = 0; i < individualsPerGeneration; i++)
            {
                scenarios[i].id = i;
            }

            StartCoroutine(InstantiateBotCoroutine(0.3f));
        }

        public override void NeuralBotDestroyed(Brain neuralBot)
        {
            base.NeuralBotDestroyed(neuralBot);

            if (count <= 0)
            {
                generation++;
                Save();
                population = Mendelization();
                StartCoroutine(InstantiateBotCoroutine(1));
            }
        }

        public void NeuralBotDestroyed(int id)
        {
            if (currentBlocks[id])
                Destroy(currentBlocks[id]);
            if (!currentNeuralBot[id])
                return;
            if (currentNeuralBot[id].gameObject.activeInHierarchy)
            {
                count--;
                currentNeuralBot[id].gameObject.SetActive(false);
                NeuralBotDestroyed(currentNeuralBot[id]);
            }

        }
        IEnumerator InstantiateBotCoroutine(float time)
        {
            count = individualsPerGeneration;
            for (int i = 0; i < count; i++)
            {
                if (currentNeuralBot[i])
                    Destroy(currentNeuralBot[i].gameObject);
            }

            yield return new WaitForSeconds(time);

            for (int i = 0; i < count; i++)
            {
                scenarios[i].ball.SendMessage("Setup");

                if (currentBlocks[i])
                    Destroy(currentBlocks[i]);
                if (currentNeuralBot[i])
                    Destroy(currentNeuralBot[i]);

                currentBlocks[i] = Instantiate(scenarios[i].blocksPrefab,
                    scenarios[i].blockPoint.position, scenarios[i].blockPoint.rotation,
                    scenarios[i].transform);

                currentNeuralBot[i] = InstantiateBot(population[index], lifeTime,
                    scenarios[i].startPoint, index);
            }
        }
    }
}