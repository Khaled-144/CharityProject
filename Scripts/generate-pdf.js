const fs = require('fs');
const html_to_pdf = require('html-pdf-node');

const contentJson = process.argv[2];
const optionsJson = process.argv[3];

const content = JSON.parse(contentJson);
const options = JSON.parse(optionsJson);

const htmlContent = `
<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
    <meta charset="UTF-8">
    <style>
        body {
            font-family: 'Arial', sans-serif;
            direction: rtl;
        }
        .field {
            position: absolute;
            font-size: 14px;
        }
        #employeeName { top: 300px; left: 400px; }
        #employeeId { top: 330px; left: 400px; }
        #salaryAmount { top: 360px; left: 400px; }
        #recipient { top: 390px; left: 400px; }
        #customRecipientName { top: 420px; left: 400px; }
    </style>
</head>
<body>
    <div class="field" id="employeeName">اسم الموظف: ${content.employeeName}</div>
    <div class="field" id="employeeId">رقم الموظف: ${content.employeeId}</div>
    <div class="field" id="salaryAmount">مبلغ الراتب: ${content.salaryAmount}</div>
    <div class="field" id="recipient">إلى: ${content.recipient}</div>
    ${content.customRecipientName ? `<div class="field" id="customRecipientName">اسم الجهة: ${content.customRecipientName}</div>` : ''}
</body>
</html>
`;

const file = { 
    content: htmlContent
};

(async () => {
    try {
        const pdfBuffer = await html_to_pdf.generatePdf(file, options);
        fs.writeFileSync(options.path, pdfBuffer);
        console.log("PDF generated successfully");
        process.exit(0);
    } catch (error) {
        console.error("Error generating PDF:", error);
        process.exit(1);
    }
})();
