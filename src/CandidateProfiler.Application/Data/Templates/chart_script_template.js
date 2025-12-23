    new Chart(document.getElementById('skillsCoverageChart{{CANDIDATE_INDEX}}'), {
        type: 'doughnut',
        data: {
            labels: ['Detected', 'Missing'],
            datasets: [{
                data: [{{KEYWORDS_DETECTED}}, {{KEYWORDS_MISSING}}],
                backgroundColor: ['#36A2EB', '#FF6384']
            }]
        },
        options: {
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return context.label + ': ' + context.raw;
                        }
                    }
                }
            }
        }
    });

    new Chart(document.getElementById('relevancePieChart{{CANDIDATE_INDEX}}'), {
        type: 'pie',
        data: {
            labels: ['Title Match', 'Responsibility Match', 'Overall Fit'],
            datasets: [{
                data: [{{TITLE_MATCH}}, {{RESPONSIBILITY_MATCH}}, {{OVERALL_FIT}}],
                backgroundColor: ['#FFCE56', '#4BC0C0', '#9966FF']
            }]
        },
        options: {
            plugins: {
                legend: { position: 'bottom' },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return context.label + ': ' + context.raw + '%';
                        }
                    }
                }
            }
        }
    });
