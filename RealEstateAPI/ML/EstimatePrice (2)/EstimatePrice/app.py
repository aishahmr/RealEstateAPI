from flask import Flask, render_template_string, request, jsonify, redirect, url_for
import joblib
import pandas as pd
import random

# Load model
model = joblib.load("C:\\Users\\DELL\\Downloads\\RealEstateAPI-main-1\\RealEstateAPI-main\\RealEstateAPI\\ML\\EstimatePrice (2)\\EstimatePrice\\FinalDataset.pkl")

# Load dataset
df = pd.read_csv("C:\\Users\\DELL\\Downloads\\RealEstateAPI-main-1\\RealEstateAPI-main\\RealEstateAPI\\ML\\EstimatePrice (2)\\EstimatePrice\\FinalDataset.csv")

# Prepare numeric fields
df['Price 2023 (EGP)'] = df['Price 2023 (EGP)'].str.replace(',', '').astype(float)
df['Price 2024 (EGP)'] = df['Price 2024 (EGP)'].str.replace(',', '').astype(float)
df['Price 2025 (EGP)'] = df['Price 2025 (EGP)'].str.replace(',', '').astype(float)

# Fill missing text fields
df['Amenities'] = df['Amenities'].fillna("")
df['Nearby Facility'] = df['Nearby Facility'].fillna("")

# Encode Property Type
df['Property Type Encoded'] = df['Property Type'].map({'Apartment': 0, 'Villa': 1})

app = Flask(__name__)
app.secret_key = 'super_secret_key'

# ---------------- HTML Template ----------------
HTML_TEMPLATE = """
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Estimate Property Price After 1 Year</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
</head>
<body class="bg-light">
    <div class="container py-5">
        <div class="text-center mb-4">
            <h1 class="display-5">🏡 Property Estimator</h1>
            <p class="text-muted">AI-powered prediction of real estate price growth</p>
        </div>

        <div class="card shadow-lg p-4 mx-auto" style="max-width: 700px;">
            <h4 class="mb-3 text-primary">Property Details</h4>
            <ul class="list-group list-group-flush mb-4">
                <li class="list-group-item">📐 <strong>Area:</strong> {{ property['Area (sqm)'] }} sqm</li>
                <li class="list-group-item">🛏 <strong>Bedrooms:</strong> {{ property['Bedrooms'] }}</li>
                <li class="list-group-item">🛁 <strong>Bathrooms:</strong> {{ property['Bathrooms'] }}</li>
                <li class="list-group-item">🏘 <strong>Type:</strong> {{ 'Apartment' if property['Property Type Encoded'] == 0 else 'Villa' }}</li>
                <li class="list-group-item">💰 <strong>Price:</strong> {{ price_2025 }} EGP</li>
                <li class="list-group-item">✨ <strong>Amenities:</strong> {{ property['Amenities'] }}</li>
                <li class="list-group-item">📍 <strong>Nearby Facilities:</strong> {{ property['Nearby Facility'] }}</li>
            </ul>

            <form method="post">
                <input type="hidden" name="row_index" value="{{ row_index }}">
                <button type="submit" class="btn btn-success w-100">🔮 Predict After 1 Year</button>
            </form>

            {% if prediction %}
                <div class="alert alert-info mt-4 text-center">
                    <h5 class="mb-0">📈 Predicted 2026 Price:</h5>
                    <h3 class="text-success">{{ prediction }} EGP</h3>
                </div>
            {% endif %}
        </div>

        <div class="text-center mt-4">
            <a href="/" class="btn btn-outline-secondary">🔁 Show Another Property</a>
        </div>
    </div>
</body>
</html>
"""

# ---------------- Web Frontend ----------------
@app.route('/', methods=['GET', 'POST'])
def home_page():
    if request.method == 'POST':
        row_index = int(request.form.get("row_index"))
        property_row = df.iloc[row_index]
        input_data = pd.DataFrame([property_row])
        prediction = model.predict(input_data)[0]
        return render_template_string(HTML_TEMPLATE,
                                      property=property_row,
                                      price_2025=property_row['Price 2025 (EGP)'],
                                      prediction=int(prediction),
                                      row_index=row_index)
    else:
        row_index = random.randint(0, len(df) - 1)
        property_row = df.iloc[row_index]
        return render_template_string(HTML_TEMPLATE,
                                      property=property_row,
                                      price_2025=property_row['Price 2025 (EGP)'],
                                      prediction=None,
                                      row_index=row_index)

# ---------------- API Endpoint ----------------
@app.route('/api/predict', methods=['POST'], endpoint="predict_api")
def api_predict():
    data = request.get_json()
    try:
        input_data = pd.DataFrame([{
            'Price 2023 (EGP)': data.get('Price 2023 (EGP)') or data['price2023'],
            'Price 2024 (EGP)': data.get('Price 2024 (EGP)') or data['price2024'],
            'Price 2025 (EGP)': data.get('Price 2025 (EGP)') or data['price2025'],
            'Area (sqm)': data.get('Area (sqm)') or data['area'],
            'Bedrooms': data['bedrooms'],
            'Bathrooms': data['bathrooms'],
            'Property Type Encoded': 0 if (data.get('Property Type') or data['propertyType']).lower() == 'apartment' else 1,
            'Amenities': data.get('Amenities') or data['amenities'],
            'Nearby Facility': data.get('Nearby Facility') or data['nearbyFacilities']
        }])
        pred_price = model.predict(input_data)[0]
        return jsonify({"predicted_price": int(pred_price)})
    except Exception as e:
        return jsonify({"error": str(e), "received_data": data}), 400

# ---------------- Run App ----------------
if __name__ == '__main__':
    app.run(debug=True, extra_files=['templates/*'])
