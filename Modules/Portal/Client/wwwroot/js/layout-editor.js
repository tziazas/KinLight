// Bridge between Blazor and gridstack.js for the Arrange page (ARCHITECTURE.md §13).
// Ownership rule: Blazor renders and removes the item elements; gridstack only moves and resizes them.
// gridstack must never create or delete DOM nodes here, or Blazor's diffing breaks.
// Requires lib/gridstack/gridstack-all.js to be loaded first (window.GridStack).

const ROWS = 6;

export function create(gridEl, frameEl, dotnet, options) {
  const grid = GridStack.init({
    column: options.columns,
    row: ROWS,
    minRow: ROWS,
    maxRow: ROWS,
    cellHeight: 60,
    margin: 4,
    float: true,
    animate: false,
    acceptWidgets: false,
    resizable: { handles: "se" },
    columnOpts: undefined,
  }, gridEl);

  const report = (items) => {
    if (!items || !items.length) return;
    const changes = items
      .filter((n) => n.el)
      .map((n) => ({ id: n.el.getAttribute("gs-id"), x: n.x, y: n.y, w: n.w, h: n.h }));
    dotnet.invokeMethodAsync("OnGridChanged", changes);
  };

  grid.on("change", (_ev, items) => report(items));
  grid.on("added", (_ev, items) => report(items));

  const fit = () => {
    const width = frameEl.clientWidth;
    if (!width) return;
    const height = width / options.aspect;
    grid.cellHeight(height / ROWS, true);
  };

  const observer = new ResizeObserver(fit);
  observer.observe(frameEl);
  fit();

  return { grid, observer, gridEl };
}

export function adopt(handle, id) {
  const el = handle.gridEl.querySelector(`[gs-id="${id}"]`);
  if (el && !el.gridstackNode) {
    handle.grid.makeWidget(el);
  }
}

export function detach(handle, id) {
  const el = handle.gridEl.querySelector(`[gs-id="${id}"]`);
  if (el && el.gridstackNode) {
    // removeDOM=false: Blazor removes the element on its next render.
    handle.grid.removeWidget(el, false, false);
  }
}

export function setAspect(handle, frameEl, aspect) {
  const width = frameEl.clientWidth;
  if (!width) return;
  handle.grid.cellHeight(width / aspect / ROWS, true);
}

export function dispose(handle) {
  handle.observer.disconnect();
  handle.grid.offAll();
  handle.grid.destroy(false);
}
