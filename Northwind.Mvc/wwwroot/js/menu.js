document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('[data-fly-cart]').forEach(button => {
    button.addEventListener('click', () => button.classList.add('ce-pulse'));
  });
});
