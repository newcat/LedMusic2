using LedMusic2.LedColors;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace LedMusic2.Nodes.NodeModels
{
    [Node("Particle", NodeCategory.GENERATOR)]
    class ParticleNode : NodeBase
    {

        private List<Particle> particles = new List<Particle>();
        private bool direction = false; //false = left; true = right
        private double particlesToSpawn = 0.0;

        private NodeInterface<bool> niEmit;
        private NodeInterface<double> niRate;
        private NodeInterface<double> niStartVelocity;
        private NodeInterface<double> niEndVelocity;
        private NodeInterface<double> niEmitterPosition;
        private NodeInterface<bool> niSymmetric;
        private NodeInterface<double> niLifetime;
        private NodeInterface<LedColor> niStartColor;
        private NodeInterface<LedColor> niEndColor;
        private NodeInterface<LedColorArray> niOutput;

        public ParticleNode() : base()
        {

            niEmit = AddInput("Emit", false);
            niRate = AddInput("Rate", 10.0);
            niStartVelocity = AddInput("Start Velocity", 0.1);
            niEndVelocity = AddInput("End Velocity", 0.05);
            niEmitterPosition = AddInput("Emitter Position", 0.0);
            niSymmetric = AddInput("Symmetric", true);
            niLifetime = AddInput("Lifetime", 30.0);
            niStartColor = AddInput<LedColor>("Start Color");
            niEndColor = AddInput<LedColor>("End Color");

            niOutput = AddOutput<LedColorArray>("Output");

        }

        public ParticleNode(JToken j) : this()
        {
            LoadState(j);
        }

        public override bool Calculate()
        {

            var toRemove = new List<Particle>();
            var resolution = GlobalProperties.Instance.Resolution;

            //update all existing particles
            particles.ForEach(p => p.CurrentLifetime++);
            particles.RemoveAll(p => p.CurrentLifetime > p.TotalLifetime || p.Position < 0 || p.Position > 1);
            particles.ForEach(p =>
            {
                double lifeProgress = p.CurrentLifetime * 1.0 / p.TotalLifetime;
                p.Color = p.StartColor.Mix(p.EndColor, (float)lifeProgress);
                p.Position += p.StartVelocity + (p.EndVelocity - p.StartVelocity) * (float)lifeProgress;
            });

            //spawn particles if necessary
            if (niEmit.Value)
            {

                var fps = GlobalProperties.Instance.FPS;
                var rate = niRate.Value / fps;
                var startVelocity = niStartVelocity.Value / fps;
                var endVelocity = niEndVelocity.Value / fps;
                var emitterPosition = niEmitterPosition.Value;
                var symmetric = niSymmetric.Value;
                var lifetimeInFrames = (int)(niLifetime.Value * fps);
                var startColor = niStartColor.Value;
                var endColor = niEndColor.Value;

                particlesToSpawn += rate;
                while (particlesToSpawn > 1.0)
                {
                    var p = new Particle()
                    {
                        TotalLifetime = lifetimeInFrames,
                        Position = emitterPosition,
                        StartVelocity = symmetric && !direction ? -startVelocity : startVelocity,
                        EndVelocity = symmetric && !direction ? -endVelocity : endVelocity,
                        Color = startColor,
                        StartColor = startColor,
                        EndColor = endColor
                    };
                    if (symmetric)
                        direction = !direction;
                    particles.Add(p);
                    particlesToSpawn--;
                }

            }

            //render the particles
            var rendered = new LedColor[resolution];
            for (int i = 0; i < resolution; i++)
                rendered[i] = new LedColor(0, 0, 0);

            foreach (var p in particles)
            {

                var left = (int)Math.Floor(p.Position * resolution);
                var right = left + 1;

                if (left >= 0 && left < resolution)
                    rendered[left] = rendered[left].Add(p.Color, (float)(1.0 - p.Position * resolution + left));

                if (right >= 0 && right < resolution)
                    rendered[right] = rendered[right].Add(p.Color, (float)(1.0 - right + p.Position * resolution));

            }

            Outputs[0].SetValue(new LedColorArray(rendered));

            return true;

        }

        private class Particle
        {

            public int CurrentLifetime { get; set; } = 0;
            public int TotalLifetime { get; set; }
            public double Position { get; set; }
            public double StartVelocity { get; set; }
            public double EndVelocity { get; set; }
            public LedColor Color { get; set; }
            public LedColor StartColor { get; set; }
            public LedColor EndColor { get; set; }

        }

    }
}
