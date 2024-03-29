using Core;
using Core.Services;
using Example;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Diagnostics;
using System.Reflection;
using Zenseless.OpenTK;

Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

var window = new GameWindow(GameWindowSettings.Default, ImmediateMode.NativeWindowSettings); // create the window
var scene = new Scene(); // create the scene
window.UpdateFrame += _ => scene.Update();
scene.AddService<IGameTime>(new GameTime(window));
scene.AddService<IInput>(new Input(window));
var collisionDetection = new CollisionDetection();
scene.AddService<ICollisionDetection>(collisionDetection);
var graphic = new Graphic();
window.RenderFrame += _ => graphic.Draw();
window.Resize += args => Graphic.Resize(args.Width, args.Height);
scene.AddService<IGraphic>(graphic);
window.RenderFrame += _ => window.SwapBuffers();

Level1.Load(scene);
//window.WindowState = WindowState.Maximized;
window.Title = Assembly.GetExecutingAssembly().GetName().Name;

window.KeyDown += args =>
{
	if (Keys.Escape == args.Key)
	{
		window.Close();
	}
};
window.Run(); // start the game loop with 60Hz
