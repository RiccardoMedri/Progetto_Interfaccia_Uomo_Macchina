import fs from "node:fs";

import path from "node:path";

import { fileURLToPath } from "node:url";

import * as sass from "sass";

import { minify } from "terser";

const scriptDir = path.dirname(fileURLToPath(import.meta.url));

const projectRoot = path.resolve(scriptDir, "..");

const cssRoot = path.join(projectRoot, "wwwroot", "css");

const jsRoot = path.join(projectRoot, "wwwroot", "js");

const tempRoot = path.join(projectRoot, "obj", "css-build");

const jsTempRoot = path.join(projectRoot, "obj", "js-build");

const routeEntries = [ "medri-base", "medri-search", "medri-detail", "medri-comparison", "medri-funnels", "medri-client", "medri-login" ];

const cssCommentPattern = new RegExp("/" + "\\*" + "[\\s\\S]*?" + "\\*" + "/", "g");

function writeFile(filePath, content) {
    fs.mkdirSync(path.dirname(filePath), {
        recursive: true
    });
    fs.writeFileSync(filePath, content);
}

function stripCssComments(content) {
    return content.replace(cssCommentPattern, "");
}

function compile(inputPath, outputPath, style) {
    const result = sass.compile(inputPath, {
        loadPaths: [ cssRoot, path.join(projectRoot, "node_modules") ],
        quietDeps: true,
        silenceDeprecations: [ "color-functions", "global-builtin", "if-function", "import" ],
        sourceMap: false,
        style: style
    });
    writeFile(outputPath, stripCssComments(result.css));
}

function compileCssEntry(entryName) {
    const inputPath = path.join(cssRoot, `${entryName}.scss`);
    const expandedPath = path.join(cssRoot, `${entryName}.css`);
    const bundlePath = path.join(cssRoot, `bundle-${entryName}.css`);
    const minBundlePath = path.join(cssRoot, `bundle-${entryName}.min.css`);
    compile(inputPath, expandedPath, "expanded");
    fs.copyFileSync(expandedPath, bundlePath);
    compile(inputPath, minBundlePath, "compressed");
}

fs.rmSync(tempRoot, {
    recursive: true,
    force: true
});

fs.rmSync(jsTempRoot, {
    recursive: true,
    force: true
});

fs.mkdirSync(tempRoot, {
    recursive: true
});

fs.mkdirSync(jsTempRoot, {
    recursive: true
});

for (const entry of routeEntries) {
    compileCssEntry(entry);
}

const adminInput = path.join(cssRoot, "medri-admin.scss");

const adminCss = path.join(cssRoot, "medri-admin.css");

compile(adminInput, adminCss, "expanded");

fs.copyFileSync(adminCss, path.join(cssRoot, "bundle-admin.css"));

compile(adminInput, path.join(cssRoot, "bundle-admin.min.css"), "compressed");

const siteInput = path.join(cssRoot, "site.scss");

const siteCss = path.join(cssRoot, "site.css");

const siteMinCss = path.join(tempRoot, "site.min.css");

compile(siteInput, siteCss, "expanded");

compile(siteInput, siteMinCss, "compressed");

const siteCssContent = fs.readFileSync(siteCss, "utf8");

const siteMinCssContent = fs.readFileSync(siteMinCss, "utf8");

writeFile(path.join(cssRoot, "bundle-global.css"), siteCssContent);

writeFile(path.join(cssRoot, "bundle-global.min.css"), siteMinCssContent);

async function buildGlobalJs() {
    const inputs = [ path.join(projectRoot, "node_modules", "bootstrap", "dist", "js", "bootstrap.bundle.js") ];
    const content = inputs.map(input => fs.readFileSync(input, "utf8")).join("\n");
    const readable = await cleanJavaScript(content, "Global JavaScript bundle");
    writeFile(path.join(jsTempRoot, "bootstrap-global.js"), readable);
    writeFile(path.join(jsRoot, "bundle-global.js"), readable);
    const minified = await minify(content, {
        compress: true,
        mangle: true,
        format: {
            comments: false
        }
    });
    if (!minified.code) {
        throw new Error("Global JavaScript bundle minification produced no output.");
    }
    writeFile(path.join(jsRoot, "bundle-global.min.js"), minified.code);
}

async function cleanJavaScript(content, label) {
    const readable = await minify(content, {
        compress: false,
        mangle: false,
        format: {
            beautify: true,
            comments: false
        }
    });
    if (!readable.code) {
        throw new Error(`${label} cleanup produced no output.`);
    }
    return readable.code;
}

async function buildVueSource() {
    const input = path.join(projectRoot, "node_modules", "vue", "dist", "vue.runtime.global.prod.js");
    const content = fs.readFileSync(input, "utf8");
    const cleaned = stripCssComments(content);
    writeFile(path.join(jsTempRoot, "vue-runtime-global.js"), cleaned);
    writeFile(path.join(jsRoot, "bundle-vue.js"), cleaned);
    writeFile(path.join(jsRoot, "bundle-vue.min.js"), cleaned);
}

async function buildJsBundle(outputName, inputNames) {
    const content = inputNames
        .map(inputName => fs.readFileSync(path.join(jsRoot, inputName), "utf8"))
        .join("\n");
    writeFile(path.join(jsRoot, outputName), content);

    const minified = await minify(content, {
        compress: true,
        mangle: true,
        format: {
            comments: false
        }
    });
    if (!minified.code) {
        throw new Error(`${outputName} minification produced no output.`);
    }

    writeFile(path.join(jsRoot, outputName.replace(/\.js$/, ".min.js")), minified.code);
}

await buildVueSource();

await buildGlobalJs();

await buildJsBundle("bundle-medri-home.js", [ "home-search.js" ]);

await buildJsBundle("bundle-medri-search-map.js", [ "search-map.js" ]);

await buildJsBundle("bundle-medri-public-actions.js", [ "public-property-actions.js" ]);

await buildJsBundle("bundle-admin-property-media.js", [ "admin-property-media.js" ]);

await buildJsBundle("bundle-admin-featured-slots.js", [ "admin-featured-slots.js" ]);

console.log("CSS and JavaScript bundles rebuilt.");
