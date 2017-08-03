// This file contains your Data Connector logic
section PQExtension1;

[DataSource.Kind="PQExtension1", Publish="PQExtension1.Publish"]
shared PQExtension1.Contents = (optional message as text) =>
    let
        _message = if (message <> null) then message else "(no message)",
        a = "Hello from PQExtension1: " & _message
    in
        a;

// Data Source Kind description
PQExtension1 = [
    Authentication = [
        // Key = [],
        // UsernamePassword = [],
        // Windows = [],
        Implicit = []
    ],
    Label = Extension.LoadString("DataSourceLabel")
];

// Data Source UI publishing description
PQExtension1.Publish = [
    Beta = true,
    Category = "Other",
    ButtonText = { Extension.LoadString("ButtonTitle"), Extension.LoadString("ButtonHelp") },
    LearnMoreUrl = "https://powerbi.microsoft.com/",
    SourceImage = PQExtension1.Icons,
    SourceTypeImage = PQExtension1.Icons
];

PQExtension1.Icons = [
    Icon16 = { Extension.Contents("PQExtension116.png"), Extension.Contents("PQExtension120.png"), Extension.Contents("PQExtension124.png"), Extension.Contents("PQExtension132.png") },
    Icon32 = { Extension.Contents("PQExtension132.png"), Extension.Contents("PQExtension140.png"), Extension.Contents("PQExtension148.png"), Extension.Contents("PQExtension164.png") }
];
