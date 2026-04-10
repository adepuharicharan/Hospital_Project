// Hospital EHR — Site JS
document.addEventListener('DOMContentLoaded', function() {
  // Sidebar toggle
  const toggle  = document.getElementById('sidebarToggle');
  const sidebar = document.getElementById('sidebar');
  const overlay = document.getElementById('sidebarOverlay');
  if (toggle && sidebar && overlay) {
    toggle.addEventListener('click', () => {
      sidebar.classList.toggle('open');
      overlay.classList.toggle('visible');
    });
    overlay.addEventListener('click', () => {
      sidebar.classList.remove('open');
      overlay.classList.remove('visible');
    });
  }

  // User dropdown
  const userBtn  = document.getElementById('userMenuBtn');
  const userDrop = document.getElementById('userDropdown');
  if (userBtn && userDrop) {
    userBtn.addEventListener('click', (e) => {
      e.stopPropagation();
      userDrop.classList.toggle('open');
    });
    document.addEventListener('click', () => userDrop.classList.remove('open'));
    userDrop.addEventListener('click', (e) => e.stopPropagation());
  }

  // Tabs
  document.querySelectorAll('.tab').forEach(tab => {
    tab.addEventListener('click', () => {
      const id = tab.dataset.tab;
      tab.closest('.tabs-wrap').querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
      tab.closest('.tabs-wrap').querySelectorAll('.tab-panel').forEach(p => p.classList.remove('active'));
      tab.classList.add('active');
      const panel = document.getElementById('tab-' + id);
      if (panel) panel.classList.add('active');
    });
  });

  // Auto-dismiss toasts
  document.querySelectorAll('.toast').forEach(t => {
    setTimeout(() => { t.style.opacity='0'; t.style.transition='opacity .5s'; setTimeout(()=>t.remove(),500); }, 5000);
  });

  // Confirm dialogs
  document.querySelectorAll('[data-confirm]').forEach(el => {
    el.addEventListener('click', (e) => {
      if (!confirm(el.dataset.confirm)) e.preventDefault();
    });
  });
});
