using Core;
using Core.Services;
using Example;
using OpenTK;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Reflection;

Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

var window = new GameWindow(); // create the window
var scene = new Scene(); // create the scene
window.UpdateFrame += (s, e) => scene.Update();
scene.AddService<IGameTime>(new GameTime(window));
scene.AddService<IInput>(new Input(window));
var collisionDetection = new CollisionDetection();
scene.AddService<ICollisionDetection>(collisionDetection);
var graphic = new Graphic();
window.RenderFrame += (s, e) => graphic.Draw();
window.Resize += (s, e) => graphic.Resize(window.Width, window.Height);
scene.AddService<IGraphic>(graphic);
window.RenderFrame += (s, e) => window.SwapBuffers();

Level1.Load(scene);
//window.WindowState = WindowState.Fullscreen; // render the window in maximized mode
window.Title = Assembly.GetExecutingAssembly().GetName().Name;

window.KeyDown += (s, a) =>
{
	if (Key.Escape == a.Key)
	{
		window.Close();
	}
};
window.Run(); // start the game loop with 60Hz
