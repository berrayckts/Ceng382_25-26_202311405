// Reusable scroll reveal helpers for the Razor frontend.
// Use .animated-section for section wrappers, .animated-text-block for copy,
// .animated-image for photos/maps, and .animated-card-grid on card containers.
(() => {
    const motionQuery = window.matchMedia('(prefers-reduced-motion: reduce)');
    const animatedSelector = '[data-scroll-animate], .animated-section, .animated-image, .animated-card-grid, .animated-text-block';
    const canAnimate = !motionQuery.matches && 'IntersectionObserver' in window;

    if (!canAnimate) {
        document.documentElement.classList.add('ce-reduced-motion');
    }

    const observer = canAnimate ? new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (!entry.isIntersecting) return;
            entry.target.classList.add('is-visible');
            observer.unobserve(entry.target);
        });
    }, {
        threshold: 0.01,
        rootMargin: '0px 0px -4% 0px'
    }) : null;

    function prepareElement(element) {
        if (element.dataset.scrollPrepared === 'true') return;
        element.dataset.scrollPrepared = 'true';
        const rect = element.getBoundingClientRect();
        if (rect.top < window.innerHeight && rect.bottom > 0) {
            element.classList.add('is-visible');
            return;
        }
        if (!observer) {
            element.classList.add('is-visible');
            return;
        }
        observer.observe(element);
    }

    function prepareStaggerGrid(grid) {
        [...grid.children].forEach((child, index) => {
            if (!(child instanceof HTMLElement) || child.dataset.scrollPrepared === 'true') return;
            child.classList.add('animated-card-item');
            child.style.setProperty('--ce-reveal-delay', `${Math.min(index * 70, 420)}ms`);
            child.dataset.scrollPrepared = 'true';
            const rect = child.getBoundingClientRect();
            if (rect.top < window.innerHeight && rect.bottom > 0) {
                child.classList.add('is-visible');
                return;
            }
            if (!observer) {
                child.classList.add('is-visible');
                return;
            }
            observer.observe(child);
        });
    }

    function refreshAnimations(root = document) {
        root.querySelectorAll(animatedSelector).forEach(element => {
            if (element.classList.contains('animated-card-grid')) {
                prepareStaggerGrid(element);
                return;
            }
            prepareElement(element);
        });
    }

    function setupGuestPriceCalculator(root = document) {
        const formatter = new Intl.NumberFormat('tr-TR', {
            style: 'currency',
            currency: 'TRY',
            maximumFractionDigits: 0
        });

        root.querySelectorAll('.guest-price-calculator').forEach(calculator => {
            if (calculator.dataset.guestCalculatorReady === 'true') return;
            calculator.dataset.guestCalculatorReady = 'true';

            const guestInput = calculator.querySelector('.js-guest-count');
            const staticGuests = calculator.querySelector('.js-guest-count-static');
            const priceEl = calculator.querySelector('.js-price-per-person');
            const totalEl = calculator.querySelector('.js-guest-total');
            const hostForm = calculator.closest('form');

            function selectedAddonsTotal() {
                if (!hostForm) return Number(calculator.dataset.selectedAddons || 0);
                return [...hostForm.querySelectorAll('input[name="SelectedOptionIds"]:checked')]
                    .reduce((sum, input) => sum + Number(input.dataset.priceDelta || 0), 0);
            }

            function update() {
                const base = Number(calculator.dataset.pricePerPerson || 0);
                const addons = selectedAddonsTotal();
                const guests = Math.max(1, Number(guestInput?.value || staticGuests?.textContent || 1));
                if (guestInput && Number(guestInput.value) < 1) {
                    guestInput.value = '1';
                }
                const perPerson = Number.isFinite(base + addons) ? base + addons : 0;
                priceEl.textContent = formatter.format(perPerson);
                totalEl.textContent = formatter.format(guests * perPerson);
            }

            guestInput?.addEventListener('input', update);
            hostForm?.addEventListener('change', update);
            update();
        });
    }

    document.addEventListener('DOMContentLoaded', () => {
        refreshAnimations();
        setupGuestPriceCalculator();

        const mutationObserver = new MutationObserver(mutations => {
            mutations.forEach(mutation => {
                mutation.addedNodes.forEach(node => {
                    if (!(node instanceof HTMLElement)) return;
                    refreshAnimations(node);
                    setupGuestPriceCalculator(node);
                    if (node.matches(animatedSelector)) {
                        refreshAnimations(node.parentElement || node);
                    }
                });
            });
        });

        mutationObserver.observe(document.body, { childList: true, subtree: true });
    });

    window.CateraAnimations = { refresh: refreshAnimations };
})();
