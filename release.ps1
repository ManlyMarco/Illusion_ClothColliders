# Config ------------------
$plugName = "ClothColliders"
$gamePrefixes = @("KK", "AI", "EC", "HS2", "KKS")

# Env setup ---------------
if ($PSScriptRoot -match '.+?\\bin\\?') {
    $dir = $PSScriptRoot + "\"
}
else {
    $dir = $PSScriptRoot + "\bin\"
}

$copy = $dir + "\copy\BepInEx" 

# Create releases ---------
function CreateZip ($element)
{
    Remove-Item -Force -Path ($dir + "\copy") -Recurse -ErrorAction SilentlyContinue
    New-Item -ItemType Directory -Force -Path ($copy + "\plugins")

    Copy-Item -Path ($dir + "\BepInEx\plugins\" + $element + "_*.*") -Destination ($copy + "\plugins\" ) -Recurse -Force 

    $copiedFiles = Get-ChildItem -Path ($copy) -Filter "*.dll" -Recurse -Force;
    if($copiedFiles)
    {
        $ver = "v" + $copiedFiles[0].VersionInfo.FileVersion.ToString()

        Compress-Archive -Path $copy -Force -CompressionLevel "Optimal" -DestinationPath ($dir + "\" + $element + "_" + $plugName + "_" + $ver + ".zip")
    }
}

foreach ($gamePrefix in $gamePrefixes) 
{
    try
    {
        CreateZip ($gamePrefix)
    }
    catch 
    {
        # retry
        CreateZip ($gamePrefix)
    }
}

Remove-Item -Force -Path ($dir + "\copy") -Recurse
