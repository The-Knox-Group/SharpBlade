﻿/* Renderer.cs
 *
 * Copyright © 2013 by Adam Hellberg and Brandon Scott.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * Disclaimer: SharpBlade is in no way affiliated
 * with Razer and/or any of its employees and/or licensors.
 * Adam Hellberg does not take responsibility for any harm caused, direct
 * or indirect, to any Razer peripherals via the use of SharpBlade.
 * 
 * "Razer" is a trademark of Razer USA Ltd.
 */

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Sharparam.SharpBlade.Razer;

namespace Sharparam.SharpBlade.Integration
{
    /// <summary>
    /// Helper class to manage rendering a WinForms form or WPF window.
    /// </summary>
    internal class Renderer : IDisposable
    {
        /// <summary>
        /// Local instance of the SwitchBlade touchpad.
        /// </summary>
        private readonly Touchpad _touchpad;

        /// <summary>
        /// WinForms Form to render.
        /// Null if no WinForms Form assigned.
        /// </summary>
        private readonly Form _form;

        /// <summary>
        /// WPF Window to render.
        /// Null if no WPF Window assigned.
        /// </summary>
        private readonly Window _window;

        /// <summary>
        /// Timer used to control rendering of form when
        /// poll mode is in use.
        /// </summary>
        private readonly Timer _winformTimer;

        /// <summary>
        /// Timer to control rendering of window when
        /// poll mode is in use.
        /// </summary>
        private readonly DispatcherTimer _wpfTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer" /> class.
        /// For rendering a WinForms form at the specified interval.
        /// </summary>
        /// <param name="touchpad">Touchpad reference.</param>
        /// <param name="form">WinForms form to render.</param>
        /// <param name="interval">The interval to render the form at,
        /// in milliseconds (MAX PRECISION = 55ms).</param>
        internal Renderer(Touchpad touchpad, Form form, int interval = 55)
        {
            _touchpad = touchpad;
            _form = form;

            _winformTimer = new Timer
            {
                Interval = interval
            };

            _winformTimer.Tick += WinformTimerOnTick;

            _winformTimer.Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer" /> class.
        /// For rendering a WPF window at the specified interval.
        /// </summary>
        /// <param name="touchpad">Touchpad reference.</param>
        /// <param name="window">WPF window to render.</param>
        /// <param name="interval">The interval to render the window at.</param>
        internal Renderer(Touchpad touchpad, Window window, TimeSpan interval)
        {
            _touchpad = touchpad;
            _window = window;

            _wpfTimer = new DispatcherTimer(interval, DispatcherPriority.Render, WpfTimerTick, Dispatcher.CurrentDispatcher);
            _wpfTimer.Start();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_winformTimer != null)
                _winformTimer.Dispose();

            if (_wpfTimer != null)
                _wpfTimer.Stop();
        }

        /// <summary>
        /// Callback for the tick event on the WinForms render timer.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void WinformTimerOnTick(object sender, EventArgs e)
        {
            _touchpad.DrawForm(_form);
        }

        /// <summary>
        /// Callback for the tick event on the WPF render timer.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void WpfTimerTick(object sender, EventArgs e)
        {
            _touchpad.DrawWindow(_window);
        }
    }
}
