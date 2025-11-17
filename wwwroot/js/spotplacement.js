// JavaScript for Spot Placement Score form
document.addEventListener('DOMContentLoaded', function() {
    const subscriptionSelect = document.getElementById('subscriptionSelect');
    const seriesSection = document.getElementById('seriesSection');
    const seriesSelect = document.getElementById('seriesSelect');
    const loadingSpinner = document.getElementById('loadingSpinner');
    const loadingText = document.getElementById('loadingText');
    const skusSection = document.getElementById('skusSection');
    const regionsSection = document.getElementById('regionsSection');
    const zonesSection = document.getElementById('zonesSection');
    const submitSection = document.getElementById('submitSection');
    const skusList = document.getElementById('skusList');
    const regionsList = document.getElementById('regionsList');
    const skuCount = document.getElementById('skuCount');
    const regionCount = document.getElementById('regionCount');
    const submitBtn = document.getElementById('submitBtn');

    const MAX_SKUS = 5;
    const MAX_REGIONS = 3;

    let selectedSkus = 0;
    let selectedRegions = 0;

    // Load instance series when subscription is selected
    subscriptionSelect.addEventListener('change', async function() {
        const subscriptionId = this.value;
        
        if (!subscriptionId) {
            hideAllSections();
            return;
        }

        // Show loading spinner
        loadingSpinner.style.display = 'block';
        loadingText.textContent = 'Loading instance series...';
        hideAllSections();

        try {
            const response = await fetch(`/Home/GetInstanceSeries?subscriptionId=${encodeURIComponent(subscriptionId)}`);
            
            if (!response.ok) {
                throw new Error('Failed to load instance series');
            }

            const data = await response.json();
            
            // Populate series dropdown
            populateSeries(data.series);
            
            // Show series section
            loadingSpinner.style.display = 'none';
            seriesSection.style.display = 'block';
            
        } catch (error) {
            console.error('Error:', error);
            alert('Failed to load instance series. Please try again.');
            loadingSpinner.style.display = 'none';
        }
    });

    // Load SKUs and regions when series is selected
    seriesSelect.addEventListener('change', async function() {
        const subscriptionId = subscriptionSelect.value;
        const series = this.value;
        
        if (!series) {
            skusSection.style.display = 'none';
            regionsSection.style.display = 'none';
            zonesSection.style.display = 'none';
            submitSection.style.display = 'none';
            return;
        }

        // Show loading spinner
        loadingSpinner.style.display = 'block';
        loadingText.textContent = 'Loading VM SKUs and Regions...';
        skusSection.style.display = 'none';
        regionsSection.style.display = 'none';
        zonesSection.style.display = 'none';
        submitSection.style.display = 'none';

        try {
            const response = await fetch(`/Home/GetSkusAndRegions?subscriptionId=${encodeURIComponent(subscriptionId)}&series=${encodeURIComponent(series)}`);
            
            if (!response.ok) {
                throw new Error('Failed to load data');
            }

            const data = await response.json();
            
            // Populate SKUs
            populateSkus(data.skus);
            
            // Populate Regions
            populateRegions(data.regions);
            
            // Show sections
            loadingSpinner.style.display = 'none';
            skusSection.style.display = 'block';
            regionsSection.style.display = 'block';
            zonesSection.style.display = 'block';
            submitSection.style.display = 'block';
            
        } catch (error) {
            console.error('Error:', error);
            alert('Failed to load VM SKUs and regions. Please try again.');
            loadingSpinner.style.display = 'none';
        }
    });

    function populateSeries(seriesList) {
        seriesSelect.innerHTML = '<option value="">-- Select an instance series --</option>';
        
        seriesList.forEach(series => {
            const option = document.createElement('option');
            option.value = series;
            option.textContent = series;
            seriesSelect.appendChild(option);
        });
    }

    function populateSkus(skus) {
        skusList.innerHTML = '';
        selectedSkus = 0;
        skuCount.textContent = '0';

        if (skus.length === 0) {
            skusList.innerHTML = '<div class="col-12"><p class="text-muted">No VM SKUs available for this series.</p></div>';
            return;
        }

        skus.forEach(sku => {
            const col = document.createElement('div');
            col.className = 'col-md-6 col-lg-4 mb-2';
            
            const checkboxDiv = document.createElement('div');
            checkboxDiv.className = 'form-check';
            
            const checkbox = document.createElement('input');
            checkbox.className = 'form-check-input sku-checkbox';
            checkbox.type = 'checkbox';
            checkbox.name = 'SelectedSkus';
            checkbox.value = sku.size;
            checkbox.id = `sku_${sku.size}`;
            
            const label = document.createElement('label');
            label.className = 'form-check-label';
            label.htmlFor = `sku_${sku.size}`;
            label.innerHTML = `<strong>${sku.name}</strong><br><small>${sku.vCpus} vCPUs, ${sku.memoryGB} GB RAM</small>`;
            
            checkboxDiv.appendChild(checkbox);
            checkboxDiv.appendChild(label);
            col.appendChild(checkboxDiv);
            skusList.appendChild(col);

            // Add change event
            checkbox.addEventListener('change', function() {
                handleSkuSelection(this);
            });
        });
    }

    function populateRegions(regions) {
        regionsList.innerHTML = '';
        selectedRegions = 0;
        regionCount.textContent = '0';

        regions.forEach(region => {
            const col = document.createElement('div');
            col.className = 'col-md-6 col-lg-4 mb-2';
            
            const checkboxDiv = document.createElement('div');
            checkboxDiv.className = 'form-check';
            
            const checkbox = document.createElement('input');
            checkbox.className = 'form-check-input region-checkbox';
            checkbox.type = 'checkbox';
            checkbox.name = 'SelectedRegions';
            checkbox.value = region.name;
            checkbox.id = `region_${region.name}`;
            
            const label = document.createElement('label');
            label.className = 'form-check-label';
            label.htmlFor = `region_${region.name}`;
            label.textContent = region.displayName;
            
            checkboxDiv.appendChild(checkbox);
            checkboxDiv.appendChild(label);
            col.appendChild(checkboxDiv);
            regionsList.appendChild(col);

            // Add change event
            checkbox.addEventListener('change', function() {
                handleRegionSelection(this);
            });
        });
    }

    function handleSkuSelection(checkbox) {
        const allSkuCheckboxes = document.querySelectorAll('.sku-checkbox');
        selectedSkus = Array.from(allSkuCheckboxes).filter(cb => cb.checked).length;
        skuCount.textContent = selectedSkus;

        // Disable unchecked boxes if max reached
        if (selectedSkus >= MAX_SKUS) {
            allSkuCheckboxes.forEach(cb => {
                if (!cb.checked) {
                    cb.disabled = true;
                }
            });
        } else {
            allSkuCheckboxes.forEach(cb => {
                cb.disabled = false;
            });
        }

        updateSubmitButton();
    }

    function handleRegionSelection(checkbox) {
        const allRegionCheckboxes = document.querySelectorAll('.region-checkbox');
        selectedRegions = Array.from(allRegionCheckboxes).filter(cb => cb.checked).length;
        regionCount.textContent = selectedRegions;

        // Disable unchecked boxes if max reached
        if (selectedRegions >= MAX_REGIONS) {
            allRegionCheckboxes.forEach(cb => {
                if (!cb.checked) {
                    cb.disabled = true;
                }
            });
        } else {
            allRegionCheckboxes.forEach(cb => {
                cb.disabled = false;
            });
        }

        updateSubmitButton();
    }

    function updateSubmitButton() {
        // Enable submit button only if at least one SKU and one region are selected
        if (selectedSkus > 0 && selectedRegions > 0) {
            submitBtn.disabled = false;
        } else {
            submitBtn.disabled = true;
        }
    }

    function hideAllSections() {
        seriesSection.style.display = 'none';
        skusSection.style.display = 'none';
        regionsSection.style.display = 'none';
        zonesSection.style.display = 'none';
        submitSection.style.display = 'none';
    }

    // Handle availability zones checkbox
    const useAvailabilityZones = document.getElementById('useAvailabilityZones');
    const useAvailabilityZonesHidden = document.getElementById('useAvailabilityZonesHidden');
    
    if (useAvailabilityZones && useAvailabilityZonesHidden) {
        useAvailabilityZones.addEventListener('change', function() {
            useAvailabilityZonesHidden.value = this.checked ? 'true' : 'false';
        });
    }

    // Disable submit button initially
    if (submitBtn) {
        submitBtn.disabled = true;
    }
});
