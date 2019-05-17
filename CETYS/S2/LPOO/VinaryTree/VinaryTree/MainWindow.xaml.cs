﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VinaryTree.classes;

namespace VinaryTree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool READY = false;

        public VBinaryTree vBinaryTree;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            READY = true;

            // Create the VBinaryTree
            vBinaryTree = new VBinaryTree(new int[] {
                11,
                20,
                4,
                5,
                -45,
                33,
                100,
                31,
                332,
                123
            });

            center_binarytree();

            // Add the VBinaryTree to the canvas
            canvas.Children.Add(vBinaryTree);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            center_binarytree();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Owner.Close();
        }

        private void input_GotFocus(object sender, RoutedEventArgs e)
        {
            if (input.Text == "Input data...")
            {
                input.Clear();
                input.Foreground = Brushes.Black;
            }
            else input.SelectAll();
        }

        private void input_LostFocus(object sender, RoutedEventArgs e)
        {
            if (input.Text == "")
            {
                input.Text = "Input data...";
                input.Foreground = Brushes.Gray;
            }
        }

        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) update();
        }

        private void view_Click(object sender, RoutedEventArgs e)
        {
            update();
        }

        private bool is_input_valid()
        {
            string[] inputs = input.Text.Split(',');
            foreach (string _input in inputs)
            {
                if (int.TryParse(_input, out int num_input))
                {
                    input.BorderBrush = input.IsFocused ? SystemColors.ActiveBorderBrush : SystemColors.InactiveBorderBrush;
                }
                else
                {
                    input.BorderBrush = Brushes.Red;
                    return false;
                }
            }

            return true;
        }

        private int[] get_input()
        {
            return input.Text.Split(',').Select(n => int.Parse(n)).ToArray();
        }

        public void center_binarytree()
        {
            if (!READY) return;

            // Center it horizontally and a little bit bellow the top of the canvas
            Canvas.SetLeft(vBinaryTree, canvas.ActualWidth / 2 - 25);
            Canvas.SetTop(vBinaryTree, 75);
        }

        public void update()
        {
            if (!READY) return;

            if (is_input_valid()) {
                int[] input = get_input();
            }
        }
    }
}
