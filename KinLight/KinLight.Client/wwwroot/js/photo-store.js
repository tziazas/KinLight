// Development-only photo blob store in IndexedDB. Metadata lives in localStorage with the rest of the fake data.
// Exposes a global so .NET can call it without a module import: window.kinlightPhotos.*
(function () {
  const DB = "kinlight.dev.photos";
  const STORE = "blobs";
  const urls = new Map();

  function open() {
    return new Promise((resolve, reject) => {
      const req = indexedDB.open(DB, 1);
      req.onupgradeneeded = () => req.result.createObjectStore(STORE);
      req.onsuccess = () => resolve(req.result);
      req.onerror = () => reject(req.error);
    });
  }

  function tx(mode, fn) {
    return open().then(db => new Promise((resolve, reject) => {
      const t = db.transaction(STORE, mode);
      const r = fn(t.objectStore(STORE));
      t.oncomplete = () => { db.close(); resolve(r && r.result); };
      t.onerror = () => { db.close(); reject(t.error); };
    }));
  }

  window.kinlightPhotos = {
    // bytes: Uint8Array from .NET
    put: (id, bytes, contentType) => tx("readwrite", s => s.put(new Blob([bytes], { type: contentType }), id)),

    url: async (id) => {
      if (urls.has(id)) return urls.get(id);
      const blob = await tx("readonly", s => s.get(id));
      if (!blob) return null;
      const u = URL.createObjectURL(blob);
      urls.set(id, u);
      return u;
    },

    remove: async (id) => {
      const u = urls.get(id);
      if (u) { URL.revokeObjectURL(u); urls.delete(id); }
      await tx("readwrite", s => s.delete(id));
    },

    // Wraps a .NET stream as a File so browser APIs (createImageBitmap) can read it.
    fileFromStream: async (streamRef, contentType) => {
      const buffer = await streamRef.arrayBuffer();
      return new File([buffer], "upload", { type: contentType });
    }
  };
})();
