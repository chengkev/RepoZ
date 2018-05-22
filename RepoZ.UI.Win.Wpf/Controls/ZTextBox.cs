﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace RepoZ.UI.Win.Wpf.Controls
{
    public class ZTextBox : TextBox
    {
		public event EventHandler Finish;

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == Key.Escape)
				Clear();

			if (FinisherKeys.Contains(e.Key))
				Finish?.Invoke(this, EventArgs.Empty);
		}

		public List<Key> FinisherKeys { get; } = new List<Key>() { Key.Down, Key.Return, Key.Enter };
	}
}
