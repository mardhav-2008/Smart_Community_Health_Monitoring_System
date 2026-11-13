
"use strict";

/* =========================================================
   COMMUNITY HEALTH INITIATIVE
   Interactions and motion system
   ========================================================= */


/* ---------------------------------------------------------
   01. HELPERS
   --------------------------------------------------------- */

const clamp = (value, min, max) =>
    Math.min(Math.max(value, min), max);

const smoothstep = (start, end, value) => {
    const t = clamp((value - start) / (end - start), 0, 1);
    return t * t * (3 - 2 * t);
};

const prefersReducedMotion = window.matchMedia(
    "(prefers-reduced-motion: reduce)"
).matches;


/* ---------------------------------------------------------
   02. MOBILE NAVIGATION
   --------------------------------------------------------- */

(() => {
    const toggle = document.querySelector(".menu-toggle");
    const navigation = document.querySelector(".nav-links");

    if (!toggle || !navigation) return;

    function closeMenu() {
        navigation.classList.remove("open");
        toggle.setAttribute("aria-expanded", "false");
    }

    toggle.addEventListener("click", () => {
        const isOpen = navigation.classList.toggle("open");

        toggle.setAttribute(
            "aria-expanded",
            String(isOpen)
        );
    });

    navigation.querySelectorAll("a").forEach(link => {
        link.addEventListener("click", closeMenu);
    });

    document.addEventListener("click", event => {
        if (
            !navigation.contains(event.target) &&
            !toggle.contains(event.target)
        ) {
            closeMenu();
        }
    });

    window.addEventListener("resize", () => {
        if (window.innerWidth > 800) {
            closeMenu();
        }
    });
})();



/* ---------------------------------------------------------
   03. GLOBAL LIQUID GLASS CURSOR LIGHT
   --------------------------------------------------------- */

(() => {
    if (
        prefersReducedMotion ||
        !window.matchMedia("(pointer: fine)").matches
    ) {
        return;
    }

    const light = document.createElement("div");
    light.className = "cursor-light";
    light.setAttribute("aria-hidden", "true");

    document.body.appendChild(light);

    let targetX = -500;
    let targetY = -500;

    let currentX = targetX;
    let currentY = targetY;

    let ticking = false;

    function animate() {
        currentX += (targetX - currentX) * 0.14;
        currentY += (targetY - currentY) * 0.14;

        light.style.transform =
            `translate3d(${currentX}px, ${currentY}px, 0) translate(-50%, -50%)`;

        if (
            Math.abs(targetX - currentX) > 0.1 ||
            Math.abs(targetY - currentY) > 0.1
        ) {
            requestAnimationFrame(animate);
        } else {
            ticking = false;
        }
    }

    function moveLight(event) {
        targetX = event.clientX;
        targetY = event.clientY;

        light.classList.add("visible");

        if (!ticking) {
            ticking = true;
            requestAnimationFrame(animate);
        }
    }

    window.addEventListener("pointermove", moveLight, {
        passive: true
    });

    document.documentElement.addEventListener("pointerleave", () => {
        light.classList.remove("visible");
    });
})();



/* ---------------------------------------------------------
   04. HERO PARALLAX
   --------------------------------------------------------- */

/*
   The background moves faster than the page foreground.

   0.6 = background receives an additional 60% of the
   foreground's scroll displacement.
*/

(() => {
    const hero = document.querySelector(".hero");
    const background = document.querySelector(".hero-background");

    if (!hero || !background || prefersReducedMotion) return;

    const PARALLAX_SPEED = 0.6;

    let ticking = false;

    function updateParallax() {
        const rect = hero.getBoundingClientRect();

        if (rect.bottom > 0 && rect.top < window.innerHeight) {
            const heroProgress = clamp(
                -rect.top / hero.offsetHeight,
                0,
                1
            );

            const offset =
                heroProgress * hero.offsetHeight * PARALLAX_SPEED;

            background.style.setProperty(
                "--hero-parallax",
                `${-offset}px`
            );
        }

        ticking = false;
    }

    function requestUpdate() {
        if (!ticking) {
            requestAnimationFrame(updateParallax);
            ticking = true;
        }
    }

    window.addEventListener("scroll", requestUpdate, {
        passive: true
    });

    window.addEventListener("resize", requestUpdate);

    updateParallax();
})();


/* ---------------------------------------------------------
   05. CONTINUOUS IMAGE EXPANSION
   --------------------------------------------------------- */

/*
   The mission photograph is one DOM element.

   It begins in a framed position on the right, then expands
   to cover the entire viewport as the user scrolls.

   Mission copy fades out.
   The next message fades in over the SAME photograph.

   No image replacement or duplicate image is used.
*/

(() => {
    const story = document.querySelector(".story");

    if (!story) return;

    const frame = story.querySelector(".story-image-frame");
    const image = story.querySelector(".story-image");
    const intro = story.querySelector(".story-intro");
    const reveal = story.querySelector(".story-reveal");
    const overlay = story.querySelector(".story-stage");
    const caption = story.querySelector(".story-caption");
    const imageLabel = story.querySelector(".story-image-label");

    if (
        !frame ||
        !image ||
        !intro ||
        !reveal ||
        !overlay ||
        !caption
    ) {
        return;
    }

    if (prefersReducedMotion) return;

    let ticking = false;

    function updateStory() {
        const rect = story.getBoundingClientRect();

        const scrollRange =
            story.offsetHeight - window.innerHeight;

        if (scrollRange <= 0) return;

        const progress = clamp(
            -rect.top / scrollRange,
            0,
            1
        );

        /*
           Interpolate the frame from its original
           editorial position to full viewport.
        */

        const insetProgress = 1 - progress;

        const mobile = window.innerWidth <= 800;

        const startTop = mobile ? 52 : 12;
        const startLeft = mobile ? 7 : 58;
        const startRight = mobile ? 7 : 8;
        const startBottom = mobile ? 8 : 12;

        frame.style.setProperty(
            "--story-top",
            `${startTop * insetProgress}vh`
        );

        frame.style.setProperty(
            "--story-left",
            `${startLeft * insetProgress}vw`
        );

        frame.style.setProperty(
            "--story-right",
            `${startRight * insetProgress}vw`
        );

        frame.style.setProperty(
            "--story-bottom",
            `${startBottom * insetProgress}vh`
        );

        frame.style.setProperty(
            "--story-radius",
            `${18 * insetProgress}px`
        );

        /*
           Gentle image zoom adds depth without
           replacing or rebuilding the photograph.
        */

        image.style.setProperty(
            "--story-image-scale",
            (1.04 + progress * 0.10).toFixed(3)
        );

        /*
           Fade mission copy as the image expands.
        */

        const introOpacity =
            1 - smoothstep(0.08, 0.36, progress);

        intro.style.opacity = introOpacity;

        /*
           Reveal the next-screen message over the
           expanded image.
        */

        const revealProgress =
            smoothstep(0.48, 0.82, progress);

        reveal.style.opacity = revealProgress;

        reveal.style.transform =
            `translateY(${35 * (1 - revealProgress)}px)`;

        reveal.style.pointerEvents =
            revealProgress > 0.9 ? "auto" : "none";

        /*
           Gradually darken the image so the white
           typography remains readable.
        */

        const overlayOpacity =
            0.05 + revealProgress * 0.75;

        overlay.style.setProperty(
            "--story-overlay-opacity",
            overlayOpacity
        );

        caption.style.opacity = revealProgress;

        if (imageLabel) {
            imageLabel.style.opacity =
                1 - smoothstep(0.15, 0.45, progress);
        }

        ticking = false;
    }

    function requestUpdate() {
        if (!ticking) {
            requestAnimationFrame(updateStory);
            ticking = true;
        }
    }

    window.addEventListener("scroll", requestUpdate, {
        passive: true
    });

    window.addEventListener("resize", requestUpdate);

    updateStory();
})();


/* ---------------------------------------------------------
   06. IMAGE LOADING FALLBACK
   --------------------------------------------------------- */

document.querySelectorAll("img").forEach(image => {
    image.addEventListener("error", () => {
        image.classList.add("image-load-error");
    });
});
