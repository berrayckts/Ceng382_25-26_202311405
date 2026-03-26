document.addEventListener("DOMContentLoaded", () => {
  const loadGalleryButton = document.getElementById("loadGalleryBtn");
  const gallery = document.getElementById("userGallery");
  const galleryState = document.getElementById("galleryState");

  if (!loadGalleryButton || !gallery || !galleryState) {
    return;
  }

  loadGalleryButton.addEventListener("click", () => {
    const hasItems = gallery.children.length > 0;

    if (!hasItems) {
      galleryState.textContent = "There are no user images available right now.";
      return;
    }

    gallery.hidden = false;
    galleryState.hidden = true;
    loadGalleryButton.textContent = "Gallery Loaded";
    loadGalleryButton.disabled = true;
  });
});
