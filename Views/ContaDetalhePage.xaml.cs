using DinDinPro.Universal.UserControls;
using DinDinPro.Universal.ViewModels;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace DinDinPro.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContaDetalhePage : Page
    {
        GestureRecognizer recognizer;
        ManipulationInputProcessor manipulationProcessor;

        ContaDetalhePageViewModel _viewModel;

        public ContaDetalhePage()
        {
            this.InitializeComponent();
            
            _viewModel = DataContext as ContaDetalhePageViewModel;

            PeriodoStackPanel.DoubleTapped += async (sender, e) =>
                {
                    e.Handled = true;
                    _viewModel.Periodo = DateTime.Now;
                    await _viewModel.CarregarLancamentos();
                };
           

            //// Create a GestureRecognizer which will be used to process the manipulations
            //// done on the rectangle
            //recognizer = new GestureRecognizer();

            //// Create a ManipulationInputProcessor which will listen for events on the
            //// rectangle, process them, and update the rectangle's position, size, and rotation
            ////manipulationProcessor = new ManipulationInputProcessor(recognizer, manipulateMe, mainCanvas);
            //manipulationProcessor = new ManipulationInputProcessor(recognizer, this.LancamentosListView, this.LayoutRoot, _viewModel);
            //manipulationProcessor.LockToXAxis();
            //manipulationProcessor.UseInertia(true);
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _viewModel.Parametro = e.Parameter;
            _viewModel.ModoNavegacao = e.NavigationMode;
        }
    }

    class ManipulationInputProcessor
    {

        double xInicio = 0;
        GestureRecognizer recognizer;
        UIElement element;
        UIElement reference;
        TransformGroup cumulativeTransform;
        MatrixTransform previousTransform;
        CompositeTransform deltaTransform;
        ContaDetalhePageViewModel _viewModel;

        public ManipulationInputProcessor(GestureRecognizer gestureRecognizer, UIElement target, UIElement referenceFrame, ContaDetalhePageViewModel viewModel)
        {
            recognizer = gestureRecognizer;
            element = target;
            reference = referenceFrame;
            _viewModel = viewModel;

            // Initialize the transforms that will be used to manipulate the shape
            InitializeTransforms();

            // The GestureSettings property dictates what manipulation events the
            // Gesture Recognizer will listen to.  This will set it to a limited
            // subset of these events.
            recognizer.GestureSettings = GenerateDefaultSettings();

            // Set up pointer event handlers. These receive input events that are used by the gesture recognizer.
            element.PointerPressed += OnPointerPressed;
            element.PointerMoved += OnPointerMoved;
            element.PointerReleased += OnPointerReleased;
            element.PointerCanceled += OnPointerCanceled;

            // Set up event handlers to respond to gesture recognizer output
            recognizer.ManipulationStarted += OnManipulationStarted;
            recognizer.ManipulationUpdated += OnManipulationUpdated;
            recognizer.ManipulationCompleted += OnManipulationCompleted;
            recognizer.ManipulationInertiaStarting += OnManipulationInertiaStarting;
            recognizer.CrossSliding += Recognizer_CrossSliding;
        }

        private async void Recognizer_CrossSliding(GestureRecognizer sender, CrossSlidingEventArgs args)
        {
            if (args.CrossSlidingState == CrossSlidingState.Started)
            {
                xInicio = args.Position.X;
                Debug.WriteLine(String.Format("Inicio: X {0}, Y {1}", args.Position.X, args.Position.Y));
            }
            else if (args.CrossSlidingState == CrossSlidingState.Completed)
            {
                Debug.WriteLine(String.Format("Fim: X {0}, Y {1}", args.Position.X, args.Position.Y));

                if (sender.CrossSlideHorizontally)
                {
                    if (xInicio > args.Position.X) //Estamos indo da direita para esquerda
                    {
                        _viewModel.Periodo = _viewModel.Periodo.AddMonths(-1);
                        await _viewModel.CarregarLancamentos();
                    }
                    else
                    {
                        _viewModel.Periodo = _viewModel.Periodo.AddMonths(1);
                        await _viewModel.CarregarLancamentos();
                    }
                }

            }
        }

        public void InitializeTransforms()
        {
            cumulativeTransform = new TransformGroup();
            deltaTransform = new CompositeTransform();
            previousTransform = new MatrixTransform() { Matrix = Matrix.Identity };

            cumulativeTransform.Children.Add(previousTransform);
            cumulativeTransform.Children.Add(deltaTransform);

            element.RenderTransform = cumulativeTransform;
        }

        // Return the default GestureSettings for this sample
        GestureSettings GenerateDefaultSettings()
        {
            return GestureSettings.ManipulationTranslateX |
                GestureSettings.ManipulationTranslateY |
                GestureSettings.ManipulationRotate |
                GestureSettings.ManipulationTranslateInertia |
                GestureSettings.ManipulationRotateInertia;
        }

        // Route the pointer pressed event to the gesture recognizer.
        // The points are in the reference frame of the canvas that contains the rectangle element.
        void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            // Set the pointer capture to the element being interacted with so that only it
            // will fire pointer-related events
            element.CapturePointer(args.Pointer);

            // Feed the current point into the gesture recognizer as a down event
            recognizer.ProcessDownEvent(args.GetCurrentPoint(reference));
        }

        // Route the pointer moved event to the gesture recognizer.
        // The points are in the reference frame of the canvas that contains the rectangle element.
        void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            // Feed the set of points into the gesture recognizer as a move event
            recognizer.ProcessMoveEvents(args.GetIntermediatePoints(reference));
        }

        // Route the pointer released event to the gesture recognizer.
        // The points are in the reference frame of the canvas that contains the rectangle element.
        void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            // Feed the current point into the gesture recognizer as an up event
            recognizer.ProcessUpEvent(args.GetCurrentPoint(reference));

            // Release the pointer
            element.ReleasePointerCapture(args.Pointer);
        }

        // Route the pointer canceled event to the gesture recognizer.
        // The points are in the reference frame of the canvas that contains the rectangle element.
        void OnPointerCanceled(object sender, PointerRoutedEventArgs args)
        {
            recognizer.CompleteGesture();
            element.ReleasePointerCapture(args.Pointer);
        }

        // When a manipulation begins, change the color of the object to reflect
        // that a manipulation is in progress
        void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            xInicio = e.Position.X;
            //Border b = element as Border;
            //b.Background = new SolidColorBrush(Windows.UI.Colors.DeepSkyBlue);
        }

        // Process the change resulting from a manipulation
        void OnManipulationUpdated(object sender, ManipulationUpdatedEventArgs e)
        {
            previousTransform.Matrix = cumulativeTransform.Value;

            // Get the center point of the manipulation for rotation
            Point center = new Point(e.Position.X, e.Position.Y);
            deltaTransform.CenterX = center.X;
            deltaTransform.CenterY = center.Y;

            // Look at the Delta property of the ManipulationDeltaRoutedEventArgs to retrieve
            // the rotation, X, and Y changes
            deltaTransform.Rotation = e.Delta.Rotation;
            deltaTransform.TranslateX = e.Delta.Translation.X;
            deltaTransform.TranslateY = e.Delta.Translation.Y;
        }

        // When a manipulation that's a result of inertia begins, change the color of the
        // the object to reflect that inertia has taken over
        void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            //Border b = element as Border;
            //b.Background = new SolidColorBrush(Windows.UI.Colors.RoyalBlue);
        }

        // When a manipulation has finished, reset the color of the object
        async void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {

            if (xInicio > e.Position.X) //Estamos indo da direita para esquerda
            {
                _viewModel.Periodo = _viewModel.Periodo.AddMonths(-1);
                await _viewModel.CarregarLancamentos();
            }
            else
            {
                _viewModel.Periodo = _viewModel.Periodo.AddMonths(1);
                await _viewModel.CarregarLancamentos();
            }
            //Border b = element as Border;
            //b.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
        }

        // Modify the GestureSettings property to only allow movement on the X axis
        public void LockToXAxis()
        {
            recognizer.CompleteGesture();
            recognizer.GestureSettings |= GestureSettings.ManipulationTranslateY | GestureSettings.ManipulationTranslateX;
            recognizer.GestureSettings ^= GestureSettings.ManipulationTranslateY;
        }

        // Modify the GestureSettings property to only allow movement on the Y axis
        public void LockToYAxis()
        {
            recognizer.CompleteGesture();
            recognizer.GestureSettings |= GestureSettings.ManipulationTranslateY | GestureSettings.ManipulationTranslateX;
            recognizer.GestureSettings ^= GestureSettings.ManipulationTranslateX;
        }

        // Modify the GestureSettings property to allow movement on both the the X and Y axes
        public void MoveOnXAndYAxes()
        {
            recognizer.CompleteGesture();
            recognizer.GestureSettings |= GestureSettings.ManipulationTranslateX | GestureSettings.ManipulationTranslateY;
        }

        // Modify the GestureSettings property to enable or disable inertia based on the passed-in value
        public void UseInertia(bool inertia)
        {
            if (!inertia)
            {
                recognizer.CompleteGesture();
                recognizer.GestureSettings ^= GestureSettings.ManipulationTranslateInertia | GestureSettings.ManipulationRotateInertia;
            }
            else
            {
                recognizer.GestureSettings |= GestureSettings.ManipulationTranslateInertia | GestureSettings.ManipulationRotateInertia;
            }
        }

        public void Reset()
        {
            element.RenderTransform = null;
            recognizer.CompleteGesture();
            InitializeTransforms();
            recognizer.GestureSettings = GenerateDefaultSettings();
        }
    }
}
