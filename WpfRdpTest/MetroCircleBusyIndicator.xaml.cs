using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ApolloUserControlLibrary
{
    public class MetroEase : EasingFunctionBase
    {
        const double x1 = 0.3;
        const double alpha = 2.1 / 3.0;
        const double scale = (1.0 - x1 / alpha) / ((1 - alpha) * (1 - alpha));

        public MetroEase()
        {
        }

        protected override Freezable CreateInstanceCore()
        {
            return new MetroEase();
        }

        protected override double EaseInCore(double normalizedTime)
        {
            double time = 1 - normalizedTime;
            double pos = 0;

            if(time < alpha)
            {
                pos = time * x1 / alpha;
            }
            else
            {
                double time2 = time - alpha;
                pos = scale * time2*time2 + time * x1 / alpha;
            }

            return 1 - pos;
        }
    }

    public class Animation
    {
        private DoubleAnimation animation;
        private RotateTransform transform;
        private UIElement element;


        public Animation(UIElement element, TimeSpan startTime )
        {
            
            animation = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(1.5));
            animation.BeginTime = startTime;
            animation.RepeatBehavior = RepeatBehavior.Forever;
            animation.EasingFunction = new MetroEase();

            transform = new RotateTransform();
            element.RenderTransform = transform;
        }

        public void Start()
        {
            transform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        public void Stop()
        {
            transform.BeginAnimation(RotateTransform.AngleProperty, null);
        }
    }

    /// <summary>
    /// Interaction logic for MetroLoadingCircle.xaml
    /// </summary>
    public partial class MetroCircleBusyIndicator : UserControl
    {

        #region Rollover Color
        public static readonly DependencyProperty ParticleColorProperty =
        DependencyProperty.Register("ParticleColor", typeof(SolidColorBrush), typeof(MetroCircleBusyIndicator),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                new PropertyChangedCallback(OnColorChanged))
            );

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MetroCircleBusyIndicator instance = d as MetroCircleBusyIndicator;
            instance.particle1.Fill = e.NewValue as SolidColorBrush;
            instance.particle2.Fill = e.NewValue as SolidColorBrush;
            instance.particle3.Fill = e.NewValue as SolidColorBrush;
            instance.particle4.Fill = e.NewValue as SolidColorBrush;
            instance.particle5.Fill = e.NewValue as SolidColorBrush;
        }

        public SolidColorBrush ParticleColor
        {
            get { return (SolidColorBrush)GetValue(ParticleColorProperty); }
            set { SetValue(ParticleColorProperty, value); }
        }
        #endregion

        Animation a1;
        Animation a2;
        Animation a3;
        Animation a4;
        Animation a5;

        public MetroCircleBusyIndicator()
        {
            InitializeComponent();

            a1 = new Animation(particle1, TimeSpan.FromSeconds(0));
            a2 = new Animation(particle2, TimeSpan.FromSeconds(0.25));
            a3 = new Animation(particle3, TimeSpan.FromSeconds(0.5));
            a4 = new Animation(particle4, TimeSpan.FromSeconds(0.75));
            a5 = new Animation(particle5, TimeSpan.FromSeconds(1.0));

            particle1.Opacity = 0;
            particle2.Opacity = 0;
            particle3.Opacity = 0;
            particle4.Opacity = 0;
            particle5.Opacity = 0;
        }

        public void Start()
        {
            a1.Start();
            a2.Start();
            a3.Start();
            a4.Start();
            a5.Start();

            particle1.Opacity = 1;
            particle2.Opacity = 1;
            particle3.Opacity = 1;
            particle4.Opacity = 1;
            particle5.Opacity = 1;
        }

        public void Stop()
        {

            a1.Stop();
            a2.Stop();
            a3.Stop();
            a4.Stop();
            a5.Stop();

            particle1.Opacity = 0;
            particle2.Opacity = 0;
            particle3.Opacity = 0;
            particle4.Opacity = 0;
            particle5.Opacity = 0;
        }
        
    }
}
