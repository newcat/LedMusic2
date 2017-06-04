using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Helpers;
using LedMusic2.Models;
using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LedMusic2.Nodes
{
    [Node("Particle", NodeCategory.GENERATOR)]
    class ParticleNode : NodeBase
    {

        private List<Particle> particles = new List<Particle>();
        private bool direction = false; //false = left; true = right
        private double particlesToSpawn = 0.0;

        public ParticleNode(Point initPosition) : base(initPosition)
        {

            Inputs.Add(new NodeInterface<bool>("Emit", ConnectionType.BOOL, this, true));
            Inputs.Add(new NodeInterface<double>("Rate", ConnectionType.NUMBER, this, true));
            Inputs.Add(new NodeInterface<double>("Start Velocity", ConnectionType.NUMBER, this, true));
            Inputs.Add(new NodeInterface<double>("End Velocity", ConnectionType.NUMBER, this, true));
            Inputs.Add(new NodeInterface<double>("Emitter Position", ConnectionType.NUMBER, this, true));
            Inputs.Add(new NodeInterface<bool>("Symmetric", ConnectionType.BOOL, this, true));
            Inputs.Add(new NodeInterface<double>("Lifetime", ConnectionType.NUMBER, this, true));
            Inputs.Add(new NodeInterface<LedColor>("Start Color", ConnectionType.COLOR, this, true));
            Inputs.Add(new NodeInterface<LedColor>("End Color", ConnectionType.COLOR, this, true));

            Outputs.Add(new NodeInterface<LedColor[]>("Output", ConnectionType.COLOR_ARRAY, this, false));

        }

        public override bool Calculate()
        {

            var toRemove = new List<Particle>();
            var ledCount = GlobalProperties.Instance.LedCount;

            //update all existing particles
            foreach (var p in particles)
            {
                p.CurrentLifetime++;
                if (p.CurrentLifetime > p.TotalLifetime || p.Position < -1 || p.Position > ledCount)
                {
                    toRemove.Add(p);
                } else
                {
                    double lifeProgress = p.CurrentLifetime * 1.0 / p.TotalLifetime;
                    p.Color = ColorHelper.Mix(p.StartColor, p.EndColor, lifeProgress);
                    p.Position += p.StartVelocity + (p.EndVelocity - p.StartVelocity) * lifeProgress;
                }
            }

            foreach (var p in toRemove)
            {
                particles.Remove(p);
            }


            var emit = ((NodeInterface<bool>)Inputs.GetNodeInterface("Emit")).Value;
            if (emit)
            {

                var rate = ((NodeInterface<double>)Inputs.GetNodeInterface("Rate")).Value;
                var startVelocity = ((NodeInterface<double>)Inputs.GetNodeInterface("Start Velocity")).Value;
                var endVelocity = ((NodeInterface<double>)Inputs.GetNodeInterface("End Velocity")).Value;
                var emitterPosition = ((NodeInterface<double>)Inputs.GetNodeInterface("Emitter Position")).Value;
                var symmetric = ((NodeInterface<bool>)Inputs.GetNodeInterface("Symmetric")).Value;
                var lifetime = (int)((NodeInterface<double>)Inputs.GetNodeInterface("Lifetime")).Value;
                var startColor = ((NodeInterface<LedColor>)Inputs.GetNodeInterface("Start Color")).Value ?? new LedColorRGB(0, 0, 0);
                var endColor = ((NodeInterface<LedColor>)Inputs.GetNodeInterface("End Color")).Value ?? new LedColorRGB(0, 0, 0);

                particlesToSpawn += rate;
                while (particlesToSpawn > 1.0)
                {
                    var p = new Particle()
                    {
                        TotalLifetime = lifetime,
                        Position = emitterPosition,
                        StartVelocity = startVelocity,
                        EndVelocity = endVelocity,
                        Color = startColor,
                        StartColor = startColor,
                        EndColor = endColor
                    };
                    if (symmetric)
                    {
                        if (!direction)
                        {
                            p.StartVelocity = -startVelocity;
                            p.EndVelocity = -endVelocity;
                            direction = true;
                        } else
                        {
                            direction = false;
                        }
                    }
                    particles.Add(p);
                    particlesToSpawn--;
                }

            }

            //render the particles
            var rendered = new LedColor[ledCount];
            for (int i = 0; i < ledCount; i++)
                rendered[i] = new LedColorRGB(0, 0, 0);

            foreach (var p in particles)
            {

                var left = (int)Math.Floor(p.Position);
                var right = (int)Math.Ceiling(p.Position);

                if (left >= 0 && left < ledCount)
                    rendered[left] = ColorHelper.Add(rendered[left], p.Color, 1.0 - p.Position + left);

                if (right >= 0 && right < ledCount)
                    rendered[right] = ColorHelper.Add(rendered[right], p.Color, 1.0 - right + p.Position);

            }

            Outputs[0].SetValue(rendered);

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
