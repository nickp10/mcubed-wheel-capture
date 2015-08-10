To setup a project:
  1. Right click the project and choose Properties.
  2. Go to the Signing tab.
  3. Check "Sign the assembly".
  4. Click the "Choose a strong name key file" dropdown.
  5. Click "<New...>".
  6. Enter the assembly name (e.g., mCubed.LineupGenerator) as the "Key file name".
  7. Check "Protect my key file with a password".
  8. Enter "mCubed3Key" as the password and confirm password.
  9. Click OK.
  10. Check "Sign the ClickOnce manifests".
  11. Click "Select from File...".
  12. Choose the file that was created above (e.g., mCubed.LineupGenerator.pfx).
  13. Enter the "mCubed3Key" password.
  14. Go to the Publish tab.
  15. Enter the "Publishing Folder Location" of: ..\..\..\Webapps\tomcat\apps\<ASSEMBLY-NAME>\
  16. Enter the "Insallation Folder URL" of: http://mcubed.ddns.net/Apps/<ASSEMBLY-NAME>/
  17. Click "Updates...".
  18. Check "The application should check for updates".
  19. Click OK.
  20. Click "Options...".
  21. Go to the Deployment tab.
  22. Uncheck "Open deployment web page after publish".
  23. Click OK.
  24. Right click the project and choose "Unload Project".
  25. Click Yes to save the changes.
  26. Right click the project and choose "Edit <PROJECT-NAME>.cspoj".
  27. At the bottom of the file, add the following target:
        <Target Name="BeforePublish">
          <Exec Command="&quot;$(MSBuildProgramFiles32)\Microsoft SDKs\Windows\v7.0A\Bin\signtool.exe&quot; sign /f &quot;$(ProjectDir)$(AssemblyName).pfx&quot; /p mCubed3Key /v &quot;$(ProjectDir)obj\x86\$(ConfigurationName)\$(TargetFileName)&quot;" />
        </Target>
  28. Right click the project and choose "Reload Project".
  29. Click Yes to close the file in the editor and Yes to save the changes.

To create a new release:
  1. In mCubedSecondary.xaml.cs, ensure the Credits region is up to date.
  2. In mCubedSecondary.xaml, ensure the About section is up to date.
  3. Run all unit tests through Visual Studio to make sure they work.
  4. Commit all changes.
  5. Set the build mode to Release.
  6. Right click the project and choose Publish.
  7. Click Finish on the wizard to publish the build.
  8. In the Webapps directory, run the corresponding SyncToDev.bat and SyncToProd.bat files.