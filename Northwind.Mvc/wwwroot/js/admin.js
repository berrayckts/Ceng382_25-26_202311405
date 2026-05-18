document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('[data-ce-sortable] th').forEach((header, index) => {
    header.addEventListener('click', () => {
      const table = header.closest('table');
      const rows = [...table.querySelectorAll('tbody tr')];
      rows.sort((a, b) => a.children[index].innerText.localeCompare(b.children[index].innerText));
      table.querySelector('tbody').replaceChildren(...rows);
    });
  });
});
