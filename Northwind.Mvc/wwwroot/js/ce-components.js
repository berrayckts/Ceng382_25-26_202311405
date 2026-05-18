window.CeToast = (() => {
  const ensureStack = () => {
    let stack = document.querySelector('.ce-toast-stack');
    if (!stack) {
      stack = document.createElement('div');
      stack.className = 'ce-toast-stack';
      document.body.appendChild(stack);
    }
    return stack;
  };

  return {
    show({ message, type = 'info', duration = 3500 }) {
      const toast = document.createElement('div');
      toast.className = `ce-toast ce-toast-${type}`;
      toast.style.setProperty('--ce-toast-duration', `${duration}ms`);
      toast.innerHTML = `<div class="ce-toast-body">${message}</div><div class="ce-toast-progress"></div>`;
      ensureStack().appendChild(toast);
      setTimeout(() => toast.remove(), duration + 250);
      return toast;
    }
  };
})();

window.CeModal = (() => {
  const focusable = 'button,[href],input,select,textarea,[tabindex]:not([tabindex="-1"])';

  return {
    open({ title, body, confirmLabel = 'Confirm', cancelLabel = 'Cancel', onConfirm }) {
      return new Promise(resolve => {
        const previous = document.activeElement;
        const backdrop = document.createElement('div');
        backdrop.className = 'ce-modal-backdrop';
        backdrop.innerHTML = `
          <section class="ce-modal-card" role="dialog" aria-modal="true" aria-labelledby="ce-modal-title">
            <div class="p-4 border-bottom"><h2 id="ce-modal-title" class="h5 m-0">${title}</h2></div>
            <div class="p-4">${body}</div>
            <div class="p-3 border-top d-flex justify-content-end gap-2">
              <button type="button" class="btn btn-outline-secondary" data-ce-cancel>${cancelLabel}</button>
              <button type="button" class="btn btn-primary" data-ce-confirm>${confirmLabel}</button>
            </div>
          </section>`;

        const close = value => {
          backdrop.remove();
          previous?.focus?.();
          resolve(value);
        };

        backdrop.addEventListener('click', event => {
          if (event.target === backdrop || event.target.closest('[data-ce-cancel]')) close(false);
          if (event.target.closest('[data-ce-confirm]')) {
            Promise.resolve(onConfirm?.()).then(() => close(true));
          }
        });
        backdrop.addEventListener('keydown', event => {
          if (event.key === 'Escape') close(false);
          if (event.key === 'Tab') {
            const nodes = [...backdrop.querySelectorAll(focusable)];
            if (!nodes.length) return;
            const first = nodes[0];
            const last = nodes[nodes.length - 1];
            if (event.shiftKey && document.activeElement === first) {
              event.preventDefault();
              last.focus();
            } else if (!event.shiftKey && document.activeElement === last) {
              event.preventDefault();
              first.focus();
            }
          }
        });
        document.body.appendChild(backdrop);
        backdrop.querySelector('[data-ce-confirm]')?.focus();
      });
    }
  };
})();

window.CeLoader = {
  page() {
    if (document.querySelector('.ce-page-loader')) return;
    const loader = document.createElement('div');
    loader.className = 'ce-page-loader';
    loader.innerHTML = '<div class="spinner-border text-primary" role="status" aria-label="Loading"></div>';
    document.body.appendChild(loader);
  },
  inline(selector) {
    const target = document.querySelector(selector);
    if (target) target.innerHTML = '<div class="placeholder-glow"><span class="placeholder col-12 py-4"></span></div>';
  },
  clear(selector) {
    document.querySelector(selector)?.replaceChildren();
    document.querySelector('.ce-page-loader')?.remove();
  }
};

window.CeForm = {
  init(selector, options = {}) {
    document.querySelectorAll(selector).forEach(form => {
      form.addEventListener('submit', async event => {
        if (!form.checkValidity()) return;
        if (!options.onSubmit) return;
        event.preventDefault();
        const button = form.querySelector('[type="submit"]');
        const original = button?.innerHTML;
        if (button) {
          button.disabled = true;
          button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processing';
        }
        try {
          await options.onSubmit(Object.fromEntries(new FormData(form)));
        } catch (error) {
          CeToast.show({ message: error.message || 'Please check the form.', type: 'error' });
        } finally {
          if (button) {
            button.disabled = false;
            button.innerHTML = original;
          }
        }
      });
    });
  }
};

window.CeRating = {
  render(selector, { value = 0, max = 5, showLabel = true } = {}) {
    document.querySelectorAll(selector).forEach(target => {
      const rounded = Math.max(0, Math.min(max, Number(value)));
      const stars = Array.from({ length: max }, (_, index) => {
        const filled = index < Math.round(rounded);
        return `<svg width="16" height="16" viewBox="0 0 20 20" aria-hidden="true" fill="${filled ? '#f59e0b' : '#d1d5db'}"><path d="M10 1.5l2.58 5.23 5.77.84-4.18 4.07.99 5.75L10 14.68l-5.16 2.71.99-5.75-4.18-4.07 5.77-.84L10 1.5z"/></svg>`;
      }).join('');
      target.innerHTML = `<span class="d-inline-flex align-items-center gap-1">${stars}${showLabel ? `<span class="ms-1 text-muted">${rounded.toFixed(1)}</span>` : ''}</span>`;
    });
  }
};

document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('[data-ce-rating]').forEach(node => {
    CeRating.render(node, { value: node.dataset.ceRating, showLabel: node.dataset.ceRatingLabel !== 'false' });
  });
});
