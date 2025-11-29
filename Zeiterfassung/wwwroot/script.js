


// Allgemeine Navigations-Logik (Sidebar und Tabs)
document.querySelectorAll('.main-nav li').forEach(item => {
    item.addEventListener('click', function () {
        document.querySelectorAll('.main-nav li').forEach(li => li.classList.remove('active'));
        this.classList.add('active');
        const targetId = this.getAttribute('data-target');
        document.querySelectorAll('.dashboard-section').forEach(section => {
            section.style.display = 'none';
        });
        document.getElementById(targetId).style.display = 'block';
        const title = targetId === 'email-section' ? 'E-mail Dashboard' : 'Sage-Tickets Dashboard';
        document.getElementById('dashboard-title').textContent = title;
    });
});

document.querySelectorAll('.email-view-switcher .view-btn').forEach(button => {
    button.addEventListener('click', function () {
        document.querySelectorAll('.email-view-switcher .view-btn').forEach(btn => btn.classList.remove('active'));
        this.classList.add('active');
        document.querySelectorAll('.email-view').forEach(view => {
            view.style.display = 'none';
        });
        document.getElementById(this.getAttribute('data-view')).style.display = 'block';
    });
});

// ----------------------------------------------------
// MODAL & ASSIGNMENT LOGIK
// ----------------------------------------------------

let currentMailId = null;
let currentTicketId = null;

// UMGANGSFUNKTION FÜR BEIDE MODALE
function openModal(itemId, type) {
    console.log("Item ID: " + itemId + " Typ: " + type);

    if (type === 'email') {
        openEmailModal(itemId);
        document.getElementById('email-modal-overlay').style.display = 'flex';
    } else if (type === 'ticket') {
        //openTicketModal(itemId);
        document.getElementById('ticket-modal-overlay').style.display = 'flex';
    }

    document.body.classList.add('modal-open');
}

// Funktion zum Schließen beider Modale
function closeModal(type) {
    if (type === 'email') {
        document.getElementById('email-modal-overlay').style.display = 'none';
        currentMailId = null;
    } else if (type === 'ticket') {
        document.getElementById('ticket-modal-overlay').style.display = 'none';
        currentTicketId = null;
    }
   
}

// 1. E-MAIL MODAL FUNKTION (Unverändert)
function openEmailModal(mailId) {
    const mailRow = document.getElementById(mailId);
    if (!mailRow) return;

    currentMailId = mailId;

    const sender = mailRow.querySelector('td[data-label="Absender"]').textContent;
    const subject = mailRow.querySelector('td[data-label="Betreff"]').textContent;
    const date = mailRow.querySelector('td[data-label="Datum"]').textContent;
    const content = mailRow.getAttribute('data-content');
    const isAssigned = mailRow.getAttribute('data-is-assigned') === 'true';

    document.getElementById('modal-sender').textContent = sender;
    document.getElementById('modal-subject').textContent = subject;
    document.getElementById('modal-date').textContent = date;
    document.getElementById('modal-body').textContent = content;

    const assignButton = document.getElementById('assign-button');
    if (isAssigned) {
        assignButton.textContent = 'Bereits zugewiesen';
        assignButton.disabled = true;
        assignButton.style.opacity = 0.5;
        assignButton.onclick = null;
    } else {
        assignButton.textContent = '⭐ Mir zuweisen';
        assignButton.disabled = false;
        assignButton.style.opacity = 1;
        assignButton.onclick = () => assignMail(currentMailId, sender, subject);
    }
}

// 2. TICKET MODAL FUNKTION (Zeigt alle Ressourcen im Detail)
function openTicketModal(ticketId) {
    console.log("Ticket ID: "+ticketId)
    const ticketRow = document.getElementById(ticketId);
    if (!ticketRow) return;

    currentTicketId = ticketId; // ID des aktuellen Tickets speichern

    // Eingabefelder beim Öffnen leeren
    document.getElementById('new-resource').value = '';
    document.getElementById('new-vorgang').value = '';
    document.getElementById('new-bereich').value = '';
    document.getElementById('new-datum-von').value = '';
    document.getElementById('new-datum-bis').value = '';
    document.getElementById('new-stunden').value = '';

    // Daten für Modal-Header (müssen noch in den data-Attributen der Zeile gespeichert sein!)
    const matchcode = ticketRow.getAttribute('data-matchcode');
    const subject = ticketRow.getAttribute('data-betreff');
    const id = ticketRow.getAttribute('data-id');
    const termin = ticketRow.getAttribute('data-termin');
    const resourcesJson = ticketRow.getAttribute('data-resources');

    // Modal-Header setzen
    document.getElementById('ticket-modal-matchcode').textContent = matchcode;
    document.getElementById('ticket-modal-subject').textContent = subject;
    document.getElementById('ticket-modal-id').textContent = id;
    document.getElementById('ticket-modal-date').textContent = termin;

    // Ressourcen-Tabelle füllen
    const resources = JSON.parse(resourcesJson);
    const tbody = document.getElementById('ticket-resources-tbody');
    tbody.innerHTML = ''; // Vorherige Inhalte löschen

    let totalHours = 0;

    resources.forEach(res => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
                    <td data-label="Ressource">${res.ressource}</td>
                    <td data-label="Vorgang">${res.vorgang}</td>
                    <td data-label="Bereich">${res.bereich}</td>
                    <td data-label="Datum von">${res.datum_von}</td>
                    <td data-label="Datum bis">${res.datum_bis}</td>
                    <td data-label="Stunden">${parseFloat(res.stunden).toFixed(2)}</td>
                `;
        tbody.appendChild(tr);
        totalHours += parseFloat(res.stunden) || 0;
    });

    // Footer-Zeile für die Gesamtstunden (colspan ist 5, da es 6 Spalten gibt)
    const footerTr = document.createElement('tr');
    footerTr.classList.add('summary-row');
    footerTr.innerHTML = `
                 <td colspan="5" style="text-align: right; font-weight: 600;">Gesamtstunden:</td>
                 <td style="font-weight: 600;">${totalHours.toFixed(2)}</td>
             `;
    tbody.appendChild(footerTr);
}

// Logik zum Hinzufügen eines Zeiteintrags (Unverändert)
function addResourceEntry() {
    if (!currentTicketId) {
        alert('Fehler: Kein Ticket ausgewählt.');
        return;
    }

    const newEntry = {
        ressource: document.getElementById('new-resource').value.trim() || 'Unbekannt',
        vorgang: document.getElementById('new-vorgang').value.trim() || 'Unbekannt',
        bereich: document.getElementById('new-bereich').value.trim() || '',
        datum_von: document.getElementById('new-datum-von').value.trim() || '',
        datum_bis: document.getElementById('new-datum-bis').value.trim() || '',
        stunden: parseFloat(document.getElementById('new-stunden').value)
    };

    if (isNaN(newEntry.stunden) || newEntry.stunden <= 0) {
        alert('Bitte tragen Sie eine gültige Stundenzahl (größer als 0) ein.');
        return;
    }

    const ticketRow = document.getElementById(currentTicketId);
    let resources = [];
    try {
        resources = JSON.parse(ticketRow.getAttribute('data-resources'));
    } catch (e) {
        console.error('Fehler beim Parsen der Ressourcen:', e);
        resources = [];
    }

    resources.push(newEntry);
    ticketRow.setAttribute('data-resources', JSON.stringify(resources));

    openTicketModal(currentTicketId);

    alert(`Eintrag "${newEntry.ressource} (${newEntry.stunden}h)" erfolgreich hinzugefügt!`);
}

// Zuweisungs-Logik (Unverändert)
function assignMail(mailId, sender, subject) {
    const mailRow = document.getElementById(mailId);
    if (!mailRow) return;

    const assignedTbody = document.getElementById('assigned-mails-tbody');
    const newRow = mailRow.cloneNode(true);

    newRow.setAttribute('data-is-assigned', 'true');
    newRow.id = 'assigned-' + mailId;

    const statusCell = newRow.querySelector('td[data-label="Datum"]');
    if (statusCell) {
        statusCell.setAttribute('data-label', 'Status');
        statusCell.textContent = 'Neu zugewiesen';
    }

    const actionButton = newRow.querySelector('.details-btn');
    actionButton.textContent = 'Bearbeiten';
    actionButton.onclick = () => openModal(newRow.id, 'email');

    assignedTbody.prepend(newRow);
    mailRow.remove();

    closeModal('email');

    document.querySelector('[data-view="assigned-view"]').click();
}

// Globaler Close Listener für die ESC-Taste
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
        closeModal('email');
        closeModal('ticket');
    }
});

// Modal bei Klick außerhalb des Modal-Content schließen
document.getElementById('email-modal-overlay').addEventListener('click', (e) => {
    if (e.target.id === 'email-modal-overlay') {
        closeModal('email');
    }
});

