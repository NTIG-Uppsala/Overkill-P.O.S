# Create a Windows Installer for a WPF Application
This guide provides step-by-step instructions for creating an installer for your WPF application using Advanced Installer and Visual Studio.

## Prerequisites
1. **Built Solution**: Ensure your WPF project is built successfully in Visual Studio.
2. **Visual Studio Extensions**:
  - Install the **Advanced Installer** extension:
    - Navigate to ```Extensions > Manage Extensions```.
    - Search for ```Advanced Installer```.
    - Install it and restart Visual Studio as prompted.

## Step 1: Create an Installer Project
1. **Add the Installer Project**:
  - Go to ```File > New > Project```.
  - Select ```Advanced Installer Project``` from the list of templates.
  - Choose ```Add to solution``` in the solution field.
  - Provide a name for the installer project.
2. **Launch Advanced Installer**:
  - A prompt will appear to install and launch Advanced Installer.
  - Follow the on-screen prompts to install and open Advanced Installer.

## Step 2: Configure Installer in Advanced Installer
1. **Select the Application**:
  - In the Advanced Installer wizard, choose **Visual Studio Application**.
  - Select the main project solution for the WPF application.
2. **Add Necessary Files**:
  - Ensure all essential files for the WPF app, such as the ```.exe``` and required dependencies, are selected in the **Detected Files** step.
3. **Fill Product Details**:
  - Navigate to the **Product Details** section.
  - Enter the required information, including product **name**, **version**, and **publisher**.

## Step 3: Build the Installer
1. Return to the **Product Details** tab.
2. Click **Build** in the upper-left corner.
3. Locate the generated ```.msi``` file in the build output folder, accessible from the **build log**.

---

[Go to README](https://github.com/NTIG-Uppsala/Overkill-P.O.S?tab=readme-ov-file#readme)
