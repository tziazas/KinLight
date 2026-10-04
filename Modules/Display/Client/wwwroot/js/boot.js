// Boot contract between a host page and the KinLight display (ARCHITECTURE.md §11).
// Culture is fixed at startup: the runtime only loads resource assemblies for the culture it boots with, and the
// display's language comes from its configuration, not the browser. A host reads the cached config before Blazor
// starts and calls kinlightDisplay.start(culture). When the configured language later differs from the boot culture,
// the display asks shouldReloadFor(culture) and reloads once.
//
// Host usage (index.html):
//   <script src="_content/KinLight.Modules.Display.Client/js/boot.js"></script>
//   <script src="_framework/blazor.webassembly.js" autostart="false"></script>
//   <script>kinlightDisplay.start(/* culture from cached config, or null */);</script>
window.kinlightDisplay = {
    bootCulture: null,

    start: function (culture) {
        this.bootCulture = culture || null;
        Blazor.start(this.bootCulture ? { applicationCulture: this.bootCulture } : {})
            .catch(function (e) { console.error("KinLight display failed to start", e); });
    },

    // True at most once per culture per browser session, and never when the host already booted with it.
    shouldReloadFor: function (culture) {
        if (!culture) { return false; }
        if (this.bootCulture && this.bootCulture.toLowerCase() === culture.toLowerCase()) { return false; }
        try {
            var key = "kinlight.display.reloadedFor";
            if (sessionStorage.getItem(key) === culture) { return false; }
            sessionStorage.setItem(key, culture);
            return true;
        } catch (e) {
            return false;
        }
    }
};
