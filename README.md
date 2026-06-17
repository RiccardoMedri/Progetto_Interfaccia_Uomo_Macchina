# Medri Website - predisposizione tecnica

Questa cartella contiene la soluzione ASP.NET Core MVC del progetto Medri, nata dalla
base docente obbligatoria e rinominata sui nomi di dominio del prodotto. La soluzione usa
EF Core InMemory, asset Sass/Bootstrap, TypeScript/Vue mirato e bundling/minification
server-side.

## Prerequisiti verificati

- .NET SDK 10 stabile.
- Node.js/npm per dipendenze frontend del progetto Medri.
- Node.js/npm per installare anche il compilatore Sass locale dichiarato nel progetto.

## Comandi principali

Da `src`:

```powershell
dotnet restore Medri.sln
dotnet build Medri.sln --no-restore
dotnet test Medri.sln --no-build
```

Da `src/Medri.Web`:

```powershell
npm ci
npm run build:assets
```

`build:assets` compila TypeScript e SCSS usando dipendenze npm locali, senza richiedere
un'installazione globale di Sass.

## Avvio tecnico

Da `src`:

```powershell
dotnet run --project Medri.Web\Medri.Web.csproj --urls http://127.0.0.1:5058
```

## Asset e bundling

- `Medri.Web/bundleconfig.json` resta la configurazione server-side approvata per bundle/minify.
- `npm run build:assets` compila i sorgenti TypeScript e Sass nei corrispondenti asset serviti.
- `dotnet build` esegue `npm run build:assets` prima della build e prima di un eventuale target
  server-side `BundleMinify`, cosi i bundle lavorano sugli asset generati.
- Non sono stati introdotti Vite, Webpack, Parcel, esbuild o SPA Vue.
- Vue 3 e usato solo per componenti circoscritti montati in Razor Views.

## Stato test

La soluzione non contiene ancora un progetto di test dedicato. Non e stato aggiunto perche
gli ADR non approvano pacchetti di test aggiuntivi in questa predisposizione.
