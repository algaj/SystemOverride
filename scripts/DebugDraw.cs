using Godot;
using System;
using System.Collections.Generic;

namespace SpaceThing
{
    public partial class DebugDraw : MeshInstance2D
    {
        private class DebugCross
        {

            public DebugCross(Vector2 position, Color color, float scale)
            {
                Position = position;
                Color = color;
                Scale = scale;
            }

            public Vector2 Position { get; set; }
            public Color Color { get; set; }

            public float Scale { get; set; }
        };

        private class DebugLine
        {
            public DebugLine(Vector2 startPos, Vector2 endPos, Color color)
            {
                StartPos = startPos;
                EndPos = endPos;
                Color = color;
            }

            public Vector2 StartPos { get; set; }
            public Vector2 EndPos { get; set; }
            public Color Color { get; set; }
        }

        private ImmediateMesh mesh;
        private List<DebugCross> crosses = new List<DebugCross>();
        private List<DebugLine> lines = new List<DebugLine>();
        private Material material;

        public override void _Ready()
        {
            mesh = new ImmediateMesh();

            material = GD.Load<Material>("res://materials/debug_draw.tres");

            Mesh = mesh;
        }

        public override void _Process(double delta)
        {
            if (crosses.Count > 0)
            {
                mesh.ClearSurfaces();
                mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

                foreach (var cross in crosses)
                {
                    float centerOffset = cross.Scale * 0.5f;
                    mesh.SurfaceSetColor(cross.Color);
                    mesh.SurfaceAddVertex2D(cross.Position - new Vector2(centerOffset, 0.0f));
                    mesh.SurfaceAddVertex2D(cross.Position + new Vector2(centerOffset, 0.0f));
                    mesh.SurfaceAddVertex2D(cross.Position - new Vector2(0.0f, centerOffset));
                    mesh.SurfaceAddVertex2D(cross.Position + new Vector2(0.0f, centerOffset));
                }

                foreach (var line in lines)
                {
                    mesh.SurfaceSetColor(line.Color);
                    mesh.SurfaceAddVertex2D(line.StartPos);
                    mesh.SurfaceAddVertex2D(line.EndPos);
                }

                mesh.SurfaceEnd();
                crosses.Clear();
                lines.Clear();
            }
        }

        public void DrawCross(Vector2 pos, Color color, float scale)
        {
            crosses.Add(new DebugCross(pos, color, scale));
        }

        public void DrawLine(Vector2 startPos, Vector2 endPos, Color color)
        {
            lines.Add(new DebugLine(startPos, endPos, color));
        }
    }
}