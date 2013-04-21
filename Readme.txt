To create a new release:
  1. Ensure these steps are up to date.
  2. Ensure all copyright dates are up to date.
     - AssemblyInfo.cs in all the projects (mCubed, mCubed.Core, mCubed.Grabber, mCubed.Grabber.Core, and mCubed.UnitTests).
  3. In mCubedSecondary.xaml.cs, ensure the Credits region is up to date.
  4. In mCubedSecondary.xaml, ensure the About section is up to date.
  5. Run all unit tests through Visual Studio to make sure they work.
  6. Commit all changes.
  7. Set the build mode to Release and build.
  8. Copy the generated .exe and appropriate .dlls.
  9. Zip the files into an mCubed folder, commit it to the Releases directory, add it to the webapp, and deploy the webapp.