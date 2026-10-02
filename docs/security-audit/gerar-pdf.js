// Gera docs/security-audit/relatorio-auditoria-seguranca.pdf a partir de relatorio.html
// Requer: npm install puppeteer-core (em node_modules local, não instala nada globalmente)
// Usa o Microsoft Edge já instalado na máquina como motor de renderização/impressão.
//
// Uso:
//   npm install puppeteer-core
//   node gerar-pdf.js

const fs = require('fs');
const path = require('path');
const puppeteer = require('puppeteer-core');

const EDGE_CANDIDATES = [
  'C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe',
  'C:\\Program Files\\Microsoft\\Edge\\Application\\msedge.exe',
];

function encontrarEdge() {
  const encontrado = EDGE_CANDIDATES.find((p) => fs.existsSync(p));
  if (!encontrado) {
    throw new Error('Microsoft Edge não encontrado nos caminhos padrão. Ajuste EDGE_CANDIDATES em gerar-pdf.js.');
  }
  return encontrado;
}

async function main() {
  const htmlPath = path.join(__dirname, 'relatorio.html');
  const pdfPath = path.join(__dirname, 'relatorio-auditoria-seguranca.pdf');

  const browser = await puppeteer.launch({
    executablePath: encontrarEdge(),
    headless: 'new',
  });

  try {
    const page = await browser.newPage();
    await page.goto('file://' + htmlPath.replace(/\\/g, '/'), { waitUntil: 'networkidle0' });

    const headerTemplate = `
      <div style="font-size:8px; width:100%; padding:0 2cm; color:#64748B; font-family:Arial, sans-serif; display:flex; justify-content:space-between;">
        <span>Relatório de Auditoria de Segurança — OrderFlow</span>
        <span>Confidencial</span>
      </div>`;

    const footerTemplate = `
      <div style="font-size:8px; width:100%; padding:0 2cm; color:#64748B; font-family:Arial, sans-serif; display:flex; justify-content:space-between;">
        <span>OrderFlow — Auditoria de Segurança</span>
        <span>Página <span class="pageNumber"></span> de <span class="totalPages"></span></span>
      </div>`;

    await page.pdf({
      path: pdfPath,
      format: 'A4',
      printBackground: true,
      displayHeaderFooter: true,
      headerTemplate,
      footerTemplate,
      margin: { top: '2.3cm', bottom: '2.3cm', left: '2cm', right: '2cm' },
    });

    console.log('PDF gerado em:', pdfPath);
  } finally {
    await browser.close();
  }
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
