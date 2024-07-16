// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// To Show/hide Forms 
const modalToggleButton = document.getElementById('modal-toggle-button');
const modalCloseButton = document.getElementById('modal-close-button');
const modal = document.getElementById('crud-modal');
const modalOverlay = document.getElementById('modal-overlay');
const body = document.body;

modalToggleButton.addEventListener('click', () => {
    modal.classList.toggle('show');
    modalOverlay.classList.toggle('show');
    body.classList.toggle('blur-background');
});

modalCloseButton.addEventListener('click', () => {
    modal.classList.remove('show');
    modalOverlay.classList.remove('show');
    body.classList.remove('blur-background');
});
// Close modal when clicking outside of it
modalOverlay.addEventListener('click', () => {
    modal.classList.remove('show');
    modalOverlay.classList.remove('show');
    body.classList.remove('blur-background');
});