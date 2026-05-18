document.addEventListener('DOMContentLoaded', () => {
  const card = document.querySelector('[name="CardNumber"]');
  const expiry = document.querySelector('[name="Expiry"]');
  const cvv = document.querySelector('[name="Cvv"]');

  card?.addEventListener('input', () => {
    card.value = card.value.replace(/\D/g, '').slice(0, 19).replace(/(.{4})/g, '$1 ').trim();
  });
  expiry?.addEventListener('input', () => {
    const value = expiry.value.replace(/\D/g, '').slice(0, 4);
    expiry.value = value.length > 2 ? `${value.slice(0, 2)}/${value.slice(2)}` : value;
  });
  cvv?.addEventListener('input', () => {
    cvv.value = cvv.value.replace(/\D/g, '').slice(0, 4);
  });
});
