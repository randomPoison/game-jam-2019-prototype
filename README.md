# David's Cool SpatialOS Prototype Game

## Prerequisites

Before using this project, make sure to follow the [machine setup guide](https://docs.improbable.io/unity/alpha/machine-setup).

## Setup

To setup the project, you need to clone the GDK for Unity repository alongside the project so that they sit side-by-side:

```
  <common_parent_directory>
      ├── <game repo>
      ├── gdk-for-unity
```

You can use scripts to automatically do this or follow manual instructions.

### Automatic set up

From the root of the repository:

* If you are using Windows run: `powershell scripts/powershell/setup.ps1`
* If you are using Mac run: `bash scripts/shell/setup.sh`

### Manual set up

1. Clone the [GDK for Unity](https://github.com/spatialos/gdk-for-unity) repository alongside the Blank Project so that they sit side-by-side:
   * SSH - `git clone git@github.com:spatialos/gdk-for-unity.git`
   * HTTPS - `git clone https://github.com/spatialos/gdk-for-unity.git`
2. Navigate to the `gdk-for-unity` directory and checkout the pinned version which you can find in the `gdk.pinned` file, in the root of the directory.

   * `git checkout <pinned_version>`

&copy; 2019 Improbable
