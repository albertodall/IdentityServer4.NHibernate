Param(
    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string] $PublishPath,

    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string] $OutputPath
)

Add-Type -AssemblyName "$PublishPath\IdentityServer4.NHibernate.Storage.dll"
Add-Type -AssemblyName "$PublishPath\NHibernate.dll"
$configStoreOptions = New-Object IdentityServer4.NHibernate.Options.ConfigurationStoreOptions
$opStoreOptions = New-Object IdentityServer4.NHibernate.Options.OperationalStoreOptions

$bindingFlags= [Reflection.BindingFlags] "Public,Static"
$exportableSchemas = [IdentityServer4.NHibernate.Database.Databases].GetMethods($bindingFlags) `
    | Where-Object { ($_.Name -inotlike '*equals*') -and ($_.Name -inotlike '*memory*')}
$exportableSchemas | ForEach-Object {
    $fileName = $_.Name
    $currentSchema = $_.Invoke($null, $null)
    [IdentityServer4.NHibernate.Database.Schema.ScriptCreator]::CreateSchemaScriptForDatabase("$OutputPath\$fileName.sql", $currentSchema, $configStoreOptions, $opStoreOptions)
}
