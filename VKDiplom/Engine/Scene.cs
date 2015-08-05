﻿using System.Collections.Generic;
using System.Windows.Graphics;
using HermiteInterpolation.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace VKDiplom.Engine
{
    /// <summary>
    ///     Defines entire world.
    /// </summary>
    public partial class Scene
    {
        public enum LightingQuality
        {
            Low,
            Medium,
            High,
        }

        // Avoid allocating in Draw method as it is called each frame.
        private static readonly Color BackgroundColor = new Color(244, 244, 240, 255);

        private readonly Camera _camera;// = new Camera();
        // List of every shape in scene.
        private readonly List<IDrawable> _shapes = new List<IDrawable>();
        // Rotation matrix for z-axe facing up.
        private readonly Matrix _zAxeFacingUpRotation = Matrix.CreateRotationZ(MathHelper.ToRadians(-135))*
                                                        Matrix.CreateRotationX(MathHelper.ToRadians(-90));

        // Coordinate axes
        private CoordinateAxes _axes;
        // Shader used for rendering
        private BasicEffect _effect;

        private Vector3 _position = Vector3.Up;
        private Matrix _scale = Matrix.CreateScale(1.0f);
        private float _zoom = 1.0f;


        public Scene()
        {
            _camera = new Camera();
            InitializeComponent();
        }

        public Scene(Camera camera)
        {
            _camera = camera;
            InitializeComponent();
        }

        public float RotationX { get; set; }

        public float RotationY { get; set; }
        public float RotationZ { get; set; }

//        public void Rotation(Vector3 axis, float angle)
//        {
//            _rotationAxis = axis;
//            _rotationAngle = angle;
//        }
//
//        public void DefaultRotation()
//        {
//            _rotationAxis = Vector3.One;
//            _rotationAngle = 0;
//        }

        public Camera Camera
        {
            get { return _camera; }
        }

//        public List<IDrawable> Shapes
//        {
//            get { return _shapes; }
//        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                _scale = Matrix.CreateScale(_zoom);
            }
        }

        // Use advanced lighting when HW supports it.
        public bool PreferPerPixelLighting
        {
            set { _effect.PreferPerPixelLighting = value; }
        }


        private void InitializeComponent()
        {
            RotationX = 0.0f;
            RotationY = 0.0f;
            RotationZ = 0.0f;
            _axes = new CoordinateAxes();
            _effect = new BasicEffect(GraphicsDeviceManager.Current.GraphicsDevice)
            {
                VertexColorEnabled = true,
            };
            SetLighting(LightingQuality.High);
            PreferPerPixelLighting = true;
        }

        /// <summary>
        ///     Sets level of lighting. Higher values may decrease performance.
        /// </summary>
        /// <param name="quality">
        ///     Low: Basic static lighting
        ///     Medium: Dynamic lighting using one source of light.
        ///     High: Dynamic using three sources of light.
        /// </param>
        public void SetLighting(LightingQuality quality)
        {
            switch (quality)
            {
                case LightingQuality.Low:
                    LowQualityLighting();
                    break;
                case LightingQuality.Medium:
                    MediumQualityLighting();
                    break;
                case LightingQuality.High:
                    HighQualityLighting();
                    break;
            }
        }

        private void LowQualityLighting()
        {
            _effect.LightingEnabled = false;
            _effect.AmbientLightColor = new Vector3(0.8f, 0.8f, 0.8f);
            //_effect.EmissiveColor = new Vector3(0.5f, 0.4f, 0.5f);
        }

        private void MediumQualityLighting()
        {
            //_effect.EnableDefaultLighting();
            _effect.LightingEnabled = true;
            // Enable one directional light.
            _effect.DirectionalLight0.Enabled = true;
            // A light...
            _effect.DirectionalLight0.DiffuseColor = new Vector3(0.97f, 0.97f, 0.93f);
            // ... coming from...
            _effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, -1.5f, 0));
            // ... with this highlight
            _effect.DirectionalLight0.SpecularColor = new Vector3(0.8f, 0.8f, 0.8f);
            // Ambient color (i.e light coming from all directions)
            _effect.AmbientLightColor = new Vector3(0.6f, 0.6f, 0.6f);
            // Emissive color of objects (i.e. light emited from all objects)
            //_effect.EmissiveColor = new Vector3(0.5f, 0.4f, 0.5f);
        }

        private void HighQualityLighting()
        {
            //_effect.EnableDefaultLighting();
            _effect.LightingEnabled = true;
            // Enable three light sources.
            _effect.DirectionalLight0.Enabled = true;
            _effect.DirectionalLight1.Enabled = true;
            _effect.DirectionalLight2.Enabled = true;
            

            // A lights...

            // (Key light)
            _effect.DirectionalLight0.DiffuseColor = new Vector3(0.92f, 0.92f, 0.92f);
            // (Fill light)
            _effect.DirectionalLight1.DiffuseColor = new Vector3(0.32f, 0.32f, 0.32f);
            // (Rim light)
            _effect.DirectionalLight2.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);


            // ... comings from...
            _effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, -1.5f, 0));
            _effect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(-1, -1.5f, -1));
            _effect.DirectionalLight2.Direction = Vector3.Normalize(new Vector3(0, -1.5f, 1));


            // ... with this highlights
            _effect.DirectionalLight0.SpecularColor = new Vector3(0.1f, 0.1f, 0.1f);
            _effect.DirectionalLight0.SpecularColor = new Vector3(0.05f, 0.05f, 0.05f);
            _effect.DirectionalLight0.SpecularColor = new Vector3(0.09f, 0.08f, 0.08f);
            // Ambient color (i.e light coming from all directions)
            _effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
            // Emissive color of objects (i.e. light emited from all objects)
            //_effect.EmissiveColor = new Vector3(0.5f, 0.4f, 0.5f);
        }

        public void Draw()
        {
            var graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;
            // clear the existing render target
            graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, BackgroundColor, 1.0f, 0);
 
            //Define render world, view and projection.
            _effect.World = _scale
                //*Matrix.CreateFromYawPitchRoll(.01f*RotationY, .01f*RotationX, .01f*RotationZ)
                            *Matrix.CreateRotationZ(MathHelper.ToRadians(RotationZ))
                            *Matrix.CreateRotationY(MathHelper.ToRadians(RotationY))
                            *Matrix.CreateRotationX(MathHelper.ToRadians(RotationX))
                           // *Matrix.CreateFromAxisAngle(_rotationAxis, _rotationAngle)
                            //* Matrix.CreateTranslation(-1,-1,0)
                            //*Matrix.CreateTranslation(_position)
                            *_zAxeFacingUpRotation;
            _effect.View = _camera.ViewTransform;
            _effect.Projection = _camera.ProjectionTransform;


            // Apply all shader rendering passes.
            var effectTechniquePasses = _effect.CurrentTechnique.Passes;
            for (var i = 0; i < effectTechniquePasses.Count; i++)
            {
                effectTechniquePasses[i].Apply();
                for (var j = 0; j < _shapes.Count; j++)
                {
                    _shapes[j].Draw();
                } 
                _axes.Draw();
            }
            
           
        }
    }
}