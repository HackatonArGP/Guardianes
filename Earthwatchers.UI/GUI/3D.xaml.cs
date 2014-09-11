using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Earthwatchers.UI.GUI.Loaders
{
    public partial class _3D : UserControl
    {
        private EasingFunctionBase _easingFunction;

        public EasingFunctionBase EasingFunction
        {
            get { return _easingFunction; }
            set
            {
                _easingFunction = value;
                foreach (Timeline anim in animStoryboard.Children)
                {
                    if (anim is DoubleAnimation)
                    {
                        (anim as DoubleAnimation).EasingFunction = value;
                    }
                    else
                    {
                        if (anim is ColorAnimation)
                        {
                            (anim as ColorAnimation).EasingFunction = value;
                        }
                        else
                        {
                            if (anim is DoubleAnimationUsingKeyFrames)
                            {
                                foreach (DoubleKeyFrame key in (anim as DoubleAnimationUsingKeyFrames).KeyFrames)
                                {
                                    if (key is EasingDoubleKeyFrame)
                                    {
                                        (key as EasingDoubleKeyFrame).EasingFunction = value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private RepeatBehavior _repeatBehavior;

        public RepeatBehavior RepeatBehavior
        {
            get { return _repeatBehavior; }
            set
            {
                _repeatBehavior = value;
                foreach (Timeline anim in animStoryboard.Children)
                {
                    anim.RepeatBehavior = value;
                }
            }
        }

        private Duration _duration;

        public Duration Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                foreach (Timeline anim in animStoryboard.Children)
                {
                    anim.Duration = value;
                }
            }
        }

        private bool _autoReverse;

        public bool AutoReverse
        {
            get { return _autoReverse; }
            set
            {
                _autoReverse = value;
                foreach (Timeline anim in animStoryboard.Children)
                {
                    anim.AutoReverse = value;
                }
            }
        }

        private TimeSpan? _addToBeginTime;

        public TimeSpan? AddToBeginTime
        {
            get { return _addToBeginTime; }
            set
            {
                _addToBeginTime = value;
                foreach (Timeline anim in animStoryboard.Children)
                {
                    anim.BeginTime += value;
                }
            }
        }

        public _3D()
        {
            InitializeComponent();
        }
    }
}
