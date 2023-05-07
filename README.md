# Update Layout Demo
Example repo showing bug in WinUI3 where UpdateLayout does not redraw.

1. Build and run the app.
2. With the ToggleSwitch set to On (i.e. Use UpdatLayout) try varying the ArcEnd slider. Notice there is no effect on the panel.
3. Switch the ToggleSwitch set to Off (i.e. Use InvalidateMeasure and InvalidateArrange). Vary the ArcEnd slider again. Notice the panel now redraws as expected.
